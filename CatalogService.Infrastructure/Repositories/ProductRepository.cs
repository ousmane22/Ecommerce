using CatalogService.Domain.Entities;
using CatalogService.Domain.Repositories;
using CatalogService.Infrastructure.Data;
using MongoDB.Driver;

namespace CatalogService.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly MongoDbContext _context;

    public ProductRepository(MongoDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Product product)
    {
        await _context.Products.InsertOneAsync(product);
    }

    public async Task DeleteAsync(string id)
    {
        await _context.Products.DeleteOneAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsAsync(string id)
    {
       var count =  await _context.Products.CountDocumentsAsync(p => p.Id == id);
       return count > 0;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
      return await _context.Products
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
       return await _context.Products
            .Find(p => p.Category == category)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(string id)
    {
        return await _context.Products
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
    {
       var filter = Builders<Product>.Filter.Regex(p => p.Name,
           new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));

        return await _context.Products
                .Find(filter)
                .ToListAsync();
    }

    public async Task UpdateAsync(Product product)
    {
       product.UpdatedAt = DateTime.UtcNow;
       await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
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