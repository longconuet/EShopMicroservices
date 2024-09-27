using Newtonsoft.Json;
using Shopping.Web.Services.IService;
using System.Text;

namespace Shopping.Web.Services;

public class BaseService : IBaseService
{
    private readonly IHttpClientFactory _httpClientFactory;
    protected string ClientName = "";

    public BaseService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    protected HttpClient CreateClient()
    {
        return _httpClientFactory.CreateClient(ClientName);
    }


    public async Task<T> GetAsync<T>(string url)
    {
        var client = CreateClient();
        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode(); // Throws exception if status code is not 2xx

        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseData)!;
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestData)
    {
        var client = CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);

        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<TResponse>(responseData)!;
    }

    public async Task PutAsync<TRequest>(string url, TRequest requestData)
    {
        var client = CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        var response = await client.PutAsync(url, content);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string url)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync(url);

        response.EnsureSuccessStatusCode();
    }

}
