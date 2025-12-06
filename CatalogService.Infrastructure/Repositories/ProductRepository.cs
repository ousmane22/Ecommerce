#nullable disable
using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Data;
using Ecommerce.Common.Repositories.MongoDB;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CatalogService.Infrastructure.Repositories;

public class ProductRepository : BaseMongoRepository<Product>, IProductRepository
{
    private readonly MongoDbContext _context;

    public ProductRepository(
        MongoDbContext context,
        ILogger<ProductRepository> logger) : base(context.Products, logger)
    {
        _context = context;
    }

    protected override FilterDefinition<Product> BuildIdFilter(object id)
    {
        return Builders<Product>.Filter.Eq(p => p.Id, id.ToString());
    }

    protected override object GetEntityId(Product entity)
    {
        return entity.Id;
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        return await base.FindAsync(p => p.Category == category);
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
    {
        var filter = Builders<Product>.Filter.Regex(
            p => p.Name,
            new BsonRegularExpression(searchTerm, "i"));
        
        return await Collection.Find(filter).ToListAsync();
    }

    public override void Update(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        base.Update(product);
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await base.ExistsAsync(p => p.Id == id);
    }

    public async Task<bool> UpdateStockAsync(string id, int quantity)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        var update = Builders<Product>.Update
            .Inc(p => p.Stock, quantity)
            .Set(p => p.UpdatedAt, DateTime.UtcNow);

        var result = await _context.Products.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}