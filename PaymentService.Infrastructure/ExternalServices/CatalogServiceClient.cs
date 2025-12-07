using Ecommerce.Common.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Infrastructure.ExternalServices.Models;

namespace PaymentService.Infrastructure.ExternalServices;
public class CatalogServiceClient : ICatalogServiceClient
{
    private readonly IServiceHttpClient _httpClient;
    private readonly CatalogServiceSettings _settings;
    private readonly ILogger<CatalogServiceClient> _logger;

    public CatalogServiceClient(
        IServiceHttpClient httpClient,
        IOptions<CatalogServiceSettings> settings,
        ILogger<CatalogServiceClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<ProductDto> GetProductByIdAsync(string productId)
    {
        var url = $"{_settings.BaseUrl}/api/products/{productId}";
        return await _httpClient.GetAsync<ProductDto>(url);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
    {
        var url = $"{_settings.BaseUrl}/api/products/category/{category}";
        var result = await _httpClient.GetAsync<IEnumerable<ProductDto>>(url);
        return result ?? new List<ProductDto>();
    }

    public async Task<bool> CheckStockAvailabilityAsync(string productId, int requiredQuantity)
    {
        try
        {
            var product = await GetProductByIdAsync(productId);

            if (product == null)
            {
                _logger.LogWarning($"Product {productId} not found");
                return false;
            }

            var isAvailable = product.Stock >= requiredQuantity && product.IsActive;

            _logger.LogInformation(
                $"Stock check for product {productId}: Required={requiredQuantity}, Available={product.Stock}, IsAvailable={isAvailable}");

            return isAvailable;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking stock for product {productId}");
            return false;
        }
    }

    public async Task<bool> ReserveStockAsync(string productId, int quantity)
    {
        try
        {
            var url = $"{_settings.BaseUrl}/api/products/{productId}/reserve-stock";
            var request = new ReserveStockRequest { Quantity = quantity };

            var response = await _httpClient.PostAsync<ReserveStockRequest, ReserveStockResponse>(url, request);

            return response?.Success ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error reserving stock for product {productId}");
            return false;
        }
    }
}