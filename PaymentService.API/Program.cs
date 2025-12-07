using Ecommerce.Common.Http;
using Ecommerce.Common.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using PaymentService.Application.Commands;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Repositories;
using PaymentService.Infrastructure.Data;
using PaymentService.Infrastructure.ExternalServices;
using PaymentService.Infrastructure.Repositories;
using System.Linq;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register MediatR handlers from the Application assembly
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreatePaymentCommandHandler).Assembly));

// Explicitly register handler in DI
builder.Services.AddTransient<IRequestHandler<CreatePaymentCommand, PaymentDto>, CreatePaymentCommandHandler>();

// Register event publisher (No-op) for development
builder.Services.AddSingleton<IEventPublisher, NoOpEventPublisher>();

// Configure EF Core DbContext for payments
var connectionString = builder.Configuration.GetConnectionString("PaymentDb");
if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}


// Ensure connection string is not null or empty
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured. Please set 'ConnectionStrings:PaymentDb' or 'ConnectionStrings:DefaultConnection' in appsettings.json or environment variables.");
}

// Ensure migrations from the Infrastructure assembly are used
var migrationAssemblyName = typeof(PaymentDbContext).Assembly.GetName().Name;
var migrationAssembly = typeof(PaymentDbContext).Assembly;

// Log migration assembly info for debugging
Console.WriteLine($"[DEBUG] Migration Assembly: {migrationAssemblyName}");
Console.WriteLine($"[DEBUG] Found migrations in assembly {migrationAssemblyName}");

// Try to find migration types in the assembly
var migrationTypes = migrationAssembly.GetTypes()
    .Where(t => t.IsSubclassOf(typeof(Migration)) && !t.IsAbstract)
    .ToList();
Console.WriteLine($"[DEBUG] Found {migrationTypes.Count} migration type(s): {string.Join(", ", migrationTypes.Select(t => t.Name))}");

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.MigrationsAssembly(migrationAssemblyName)));

// Register payment repository
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HTTP Client pour CatalogService (Communication synchrone)
builder.Services.Configure<CatalogServiceSettings>(
    builder.Configuration.GetSection("CatalogServiceSettings"));

builder.Services.AddHttpClient<IServiceHttpClient, ServiceHttpClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Add("User-Agent", "PaymentService/1.0");
});

builder.Services.AddScoped<ICatalogServiceClient, CatalogServiceClient>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

var maskedConnString = connectionString;
if (!string.IsNullOrEmpty(maskedConnString) && maskedConnString.IndexOf("Password=", StringComparison.OrdinalIgnoreCase) >= 0)
{
    maskedConnString = System.Text.RegularExpressions.Regex.Replace(maskedConnString, "(Password=)([^;]*)", "$1****", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
}
logger.LogInformation("Using connection string: {ConnectionString}", maskedConnString);

const int maxRetries = 30;  // Increased from 10 to 30 for PostgreSQL startup
const int delaySeconds = 2;  // Wait 2 seconds between retries
for (int attempt = 1; attempt <= maxRetries; attempt++)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

        // Test connection first
        logger.LogInformation("Attempting to connect to database (attempt {Attempt}/{Max})...", attempt, maxRetries);
        
        if (!db.Database.CanConnect())
        {
            logger.LogWarning("Cannot connect to database yet. Creating database...");
            db.Database.EnsureCreated();
        }
        else
        {
            logger.LogInformation("Successfully connected to database.");
        }

        // Get migration info
        var allMigrations = db.Database.GetMigrations().ToList();
        var appliedMigrations = db.Database.GetAppliedMigrations().ToList();
        var pendingMigrations = db.Database.GetPendingMigrations().ToList();

        logger.LogInformation("Migrations - Total: {Total}, Applied: {Applied}, Pending: {Pending}",
            allMigrations.Count, appliedMigrations.Count, pendingMigrations.Count);

        // If no migrations found, manually create the Payments table
        if (allMigrations.Count == 0)
        {
            logger.LogWarning("No migrations found in assembly. Creating Payments table manually...");
            
            await db.Database.ExecuteSqlRawAsync(
                @"CREATE TABLE IF NOT EXISTS ""Payments"" (
                    ""Id"" integer NOT NULL GENERATED ALWAYS AS IDENTITY,
                    ""OrderId"" character varying(100) NOT NULL,
                    ""CustomerId"" character varying(100) NOT NULL,
                    ""Amount"" numeric(18,2) NOT NULL,
                    ""Currency"" character varying(10) NOT NULL,
                    ""Status"" integer NOT NULL,
                    ""Method"" integer NOT NULL,
                    ""TransactionId"" text NULL,
                    ""PaymentGatewayResponse"" text NULL,
                    ""CreatedAt"" timestamp with time zone NOT NULL,
                    ""ProcessedAt"" timestamp with time zone NULL,
                    ""CompletedAt"" timestamp with time zone NULL,
                    CONSTRAINT ""PK_Payments"" PRIMARY KEY (""Id"")
                )");
            
            logger.LogInformation("Payments table created successfully.");
        }
        else if (pendingMigrations.Any())
        {
            logger.LogInformation("Applying {Count} pending migration(s)...", pendingMigrations.Count);
            db.Database.Migrate();
            logger.LogInformation("Migrations applied successfully.");
        }
        else if (appliedMigrations.Count == 0)
        {
            // Migrations exist but none applied - force apply
            logger.LogWarning("Migrations exist but none are applied. Applying all migrations...");
            db.Database.Migrate();
            logger.LogInformation("All migrations applied successfully.");
        }
        else
        {
            logger.LogInformation("Database is up to date.");
        }

        // Verify Payments table exists
        try
        {
            var count = await db.Payments.CountAsync();
            logger.LogInformation("Payments table verified. Contains {Count} records.", count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Payments table verification failed after all attempts.");
            throw;
        }

        logger.LogInformation("Database initialization completed successfully.");
        break;
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Database initialization failed (attempt {Attempt}/{Max}). Retrying in {Delay}s...", 
            attempt, maxRetries, delaySeconds);
        
        if (attempt == maxRetries)
        {
            logger.LogError(ex, "Failed to initialize database after {Max} attempts. Exiting.", maxRetries);
            throw;
        }
        
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
