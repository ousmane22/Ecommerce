using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Repositories;
using Ecommerce.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerDocumentation(
    title: "Catalog Service API",
    version: "v1",
    routePrefixEmpty: true);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
