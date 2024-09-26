using Newtonsoft.Json;
using Shopping.Web.Services.IService;
using System.Text;

namespace Shopping.Web.Services;

public class BaseService : IBaseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    protected readonly HttpClient _httpClient;

    public BaseService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient("eshop-api");
    }

    public async Task<T> GetAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode(); // Throws exception if status code is not 2xx

        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseData)!;
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestData)
    {
        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponse>(responseData)!;
    }

    public async Task PutAsync<TRequest>(string url, TRequest requestData)
    {
        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(url, content);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string url)
    {
        var response = await _httpClient.DeleteAsync(url);

        response.EnsureSuccessStatusCode();
    }

}
