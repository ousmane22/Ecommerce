using CatalogService.Infrastructure.ExternalServices.Models;
using Ecommerce.Common.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CatalogService.Infrastructure.ExternalServices;

public class PaymentServiceClient : IPaymentServiceClient
{
    private readonly IServiceHttpClient _httpClient;
    private readonly PaymentServiceSettings _settings;
    private readonly ILogger<PaymentServiceClient> _logger;

    public PaymentServiceClient(
        IServiceHttpClient httpClient,
        IOptions<PaymentServiceSettings> settings,
        ILogger<PaymentServiceClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByProductIdAsync(string productId)
    {
        try
        {
            var url = $"{_settings.BaseUrl}/api/payments/product/{productId}";
            var result = await _httpClient.GetAsync<IEnumerable<PaymentDto>>(url);
            return result ?? new List<PaymentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting payments for product {productId}");
            return new List<PaymentDto>();
        }
    }

    public async Task<PaymentStatsDto> GetPaymentStatsForProductAsync(string productId)
    {
        try
        {
            var url = $"{_settings.BaseUrl}/api/payments/product/{productId}/stats";
            return await _httpClient.GetAsync<PaymentStatsDto>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting payment stats for product {productId}");
            return null;
        }
    }
}