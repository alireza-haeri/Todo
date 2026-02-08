namespace Todo.Web.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string requestUri);

    Task<TResponse?> PostAsync<TRequest, TResponse>(string requestUri, TRequest payload);

    Task<TResponse?> PutAsync<TRequest, TResponse>(string requestUri, TRequest payload);

    Task<bool> DeleteAsync(string requestUri);

    Task<string> GetStringAsync(string requestUri);
}