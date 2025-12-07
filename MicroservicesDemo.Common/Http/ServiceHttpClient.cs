
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Ecommerce.Common.Http;

public class ServiceHttpClient : IServiceHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServiceHttpClient> _logger;
    public ServiceHttpClient(HttpClient httpClient, ILogger<ServiceHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<TResponse> GetAsync<TResponse>(string url) where TResponse : class
    {
        try
        {
            _logger.LogInformation($"GET Request to: {url}");

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"GET request failed. Status: {response.StatusCode}, URL: {url}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<TResponse>();
            _logger.LogInformation($"GET Request successful: {url}");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in GET request to {url}");
            throw;
        }
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            _logger.LogInformation($"POST Request to: {url}");

            var response = await _httpClient.PostAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"POST request failed. Status: {response.StatusCode}, URL: {url}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<TResponse>();
            _logger.LogInformation($"POST Request successful: {url}");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in POST request to {url}");
            throw;
        }
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest request)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            _logger.LogInformation($"PUT Request to: {url}");

            var response = await _httpClient.PutAsJsonAsync(url, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"PUT request failed. Status: {response.StatusCode}, URL: {url}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<TResponse>();
            _logger.LogInformation($"PUT Request successful: {url}");

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in PUT request to {url}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(string url)
    {
        try
        {
            _logger.LogInformation($"DELETE Request to: {url}");

            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"DELETE request failed. Status: {response.StatusCode}, URL: {url}");
                return false;
            }

            _logger.LogInformation($"DELETE Request successful: {url}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in DELETE request to {url}");
            throw;
        }
    }
}
