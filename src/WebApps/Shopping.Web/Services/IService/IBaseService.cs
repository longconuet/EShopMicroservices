namespace Shopping.Web.Services.IService;

public interface IBaseService
{
    Task<T> GetAsync<T>(string url);
    Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestData);
    Task PutAsync<TRequest>(string url, TRequest requestData);
    Task DeleteAsync(string url);
}
