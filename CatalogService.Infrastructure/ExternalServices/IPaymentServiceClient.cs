using CatalogService.Infrastructure.ExternalServices.Models;

namespace CatalogService.Infrastructure.ExternalServices;

public interface IPaymentServiceClient
{
    Task<IEnumerable<PaymentDto>> GetPaymentsByProductIdAsync(string productId);
    Task<PaymentStatsDto> GetPaymentStatsForProductAsync(string productId);
}
