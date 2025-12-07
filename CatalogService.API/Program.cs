using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.ExternalServices;
using CatalogService.Infrastructure.Repositories;
using Ecommerce.Common.Extensions;
using Ecommerce.Common.Http;
using Ecommerce.Common.Messaging;
using MediatR;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CatalogService.Application.Commands.UpdateProductCommandHandler).Assembly));

builder.Services.AddSingleton<IEventPublisher, NoOpEventPublisher>();

// Configuration Swagger centralisée
builder.Services.AddSwaggerDocumentation(
    title: "Catalog Service API",
    version: "v1",
    description: "API pour la gestion du catalogue de produits");

// Configuration MongoDB centralisée
builder.Services.ConfigureSettings<MongoDbSettings>(
    builder.Configuration,
    "MongoDbSettings");

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddRepository<ProductRepository, IProductRepository>();

// HTTP Client pour PaymentService (Communication synchrone)
builder.Services.Configure<PaymentServiceSettings>(
    builder.Configuration.GetSection("PaymentServiceSettings"));

builder.Services.AddHttpClient<IServiceHttpClient, ServiceHttpClient>();
builder.Services.AddScoped<IPaymentServiceClient, PaymentServiceClient>();

var app = builder.Build();

app.UseSwaggerDocumentation(
    title: "Catalog Service API",
    version: "v1",
    routePrefixEmpty: true);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
