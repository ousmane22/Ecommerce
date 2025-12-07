using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Data;
using CatalogService.Infrastructure.Repositories;
using Ecommerce.Common.Extensions;
using MediatR;
using Ecommerce.Common.Messaging;

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
