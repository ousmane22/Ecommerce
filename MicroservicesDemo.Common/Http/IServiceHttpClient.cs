namespace Ecommerce.Common.Http;

public interface IServiceHttpClient
{
    Task<TResponse> GetAsync<TResponse>(string url) where TResponse : class;
    Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data)
        where TRequest : class
        where TResponse : class;
    Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data)
        where TRequest : class
        where TResponse : class;
    Task<bool> DeleteAsync(string url);
}
