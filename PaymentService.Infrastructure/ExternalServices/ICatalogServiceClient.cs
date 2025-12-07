namespace PaymentService.Infrastructure.ExternalServices;
using PaymentService.Infrastructure.ExternalServices.Models;
public interface ICatalogServiceClient
{
    Task<ProductDto> GetProductByIdAsync(string productId);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category);
    Task<bool> CheckStockAvailabilityAsync(string productId, int requiredQuantity);
    Task<bool> ReserveStockAsync(string productId, int quantity);
}
