namespace Todo.Web.Services;

/// <summary>
/// Minimal, reusable API client for the web project.
/// Keeps a small surface area while being easy to extend for more calls.
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Sends a GET request to <paramref name="requestUri"/> and deserializes the JSON response to <typeparamref name="T"/>.
    /// Returns null if the response is unsuccessful or deserialization fails.
    /// </summary>
    Task<T?> GetAsync<T>(string requestUri);

    /// <summary>
    /// Sends a POST request with JSON body and deserializes the JSON response to <typeparamref name="TResponse"/>.
    /// Returns null if the response is unsuccessful or deserialization fails.
    /// </summary>
    Task<TResponse?> PostAsync<TRequest, TResponse>(string requestUri, TRequest payload);
    
    Task<string> GetStringAsync(string requestUri);
}