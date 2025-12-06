#nullable disable
using CatalogService.Domain.Entities;
using Ecommerce.Common.Repositories;

namespace CatalogService.Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm);
    Task<bool> ExistsAsync(string id);
    Task<bool> UpdateStockAsync(string id, int quantity);
}
