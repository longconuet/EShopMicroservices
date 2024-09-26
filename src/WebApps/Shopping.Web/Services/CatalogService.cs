using Shopping.Web.Dtos;
using Shopping.Web.Services.IService;

namespace Shopping.Web.Services;

public class CatalogService : BaseService, ICatalogService
{
    public CatalogService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<List<ProductDto>> GetAllProductAsync()
    {
        var url = $"products";
        return await GetAsync<List<ProductDto>>(url);
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var url = $"products/{id}";
        return await GetAsync<ProductDto>(url);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequestDto request)
    {
        var url = $"products";
        return await PostAsync<CreateProductRequestDto, ProductDto>(url, request);
    }    

    public async Task UpdateProductAsync(UpdateProductRequestDto request)
    {
        var url = $"products";
        await PutAsync<UpdateProductRequestDto>(url, request);
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var url = $"products/{id}";
        await DeleteAsync(url);
    }
}
