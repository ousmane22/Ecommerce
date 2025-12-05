using CatalogService.Domain.Entities;

namespace CatalogService.Domain.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(string id);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<bool> UpdateStockAsync(string id, int quantity);
    Task DeleteAsync(string id);
    Task<bool> ExistAsync(string id);
}
