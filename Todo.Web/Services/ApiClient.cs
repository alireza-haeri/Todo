using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Todo.Web.Services;

public class ApiClient(HttpClient http) : IApiClient
{
    public async Task<T?> GetAsync<T>(string requestUri)
    {
        var response = await http.GetAsync(requestUri);
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string requestUri, TRequest payload)
    {
        var response = await http.PostAsJsonAsync(requestUri, payload);
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<string> GetStringAsync(string requestUri)
    {
        var response = await http.GetStringAsync(requestUri);
        return response;
    }
}