using CatalogService.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CatalogService.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        _settings = settings.Value;
        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);
    }

    public IMongoCollection<Product> Products =>
        _database.GetCollection<Product>(_settings.ProductsCollectionName);

    // Méthode pour créer les index
    public async Task CreateIndexesAsync()
    {
        var indexKeysDefinition = Builders<Product>.IndexKeys
            .Ascending(p => p.Name)
            .Ascending(p => p.Category);

        var indexModel = new CreateIndexModel<Product>(indexKeysDefinition);
        await Products.Indexes.CreateOneAsync(indexModel);
    }
}