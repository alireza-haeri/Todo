using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Todo.Web.Services;

public class ApiClient(HttpClient http, IHttpContextAccessor ctx) : IApiClient
{
    private void AddAuthHeaderIfPresent(HttpRequestMessage req)
    {
        var token = ctx.HttpContext?.Request.Cookies["access_token"];
        if (!string.IsNullOrEmpty(token))
        {
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
    public async Task<T?> GetAsync<T>(string requestUri)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        AddAuthHeaderIfPresent(req);
        var response = await http.SendAsync(req);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string requestUri, TRequest payload)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(payload)
        };
        AddAuthHeaderIfPresent(req);
        var response = await http.SendAsync(req);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string requestUri, TRequest payload)
    {
        var req = new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = JsonContent.Create(payload)
        };
        AddAuthHeaderIfPresent(req);
        var response = await http.SendAsync(req);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    public async Task<bool> DeleteAsync(string requestUri)
    {
        var req = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        AddAuthHeaderIfPresent(req);
        var response = await http.SendAsync(req);
        return response.IsSuccessStatusCode;
    }

    public async Task<string> GetStringAsync(string requestUri)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
        AddAuthHeaderIfPresent(req);
        var response = await http.SendAsync(req);
        return await response.Content.ReadAsStringAsync();
    }
}