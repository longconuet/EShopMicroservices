using Shopping.Web.Dtos;
using Shopping.Web.Services.IService;

namespace Shopping.Web.Services;

public class CatalogService : ICatalogService
{
    private readonly IBaseService _baseService;
    private readonly string _catalogApiBaseUrl = "https://localhost:5050";

    public CatalogService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<List<ProductDto>> GetAllProductAsync()
    {
        var url = $"{_catalogApiBaseUrl}/products";
        return await _baseService.GetAsync<List<ProductDto>>(url);
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var url = $"{_catalogApiBaseUrl}/products/{id}";
        return await _baseService.GetAsync<ProductDto>(url);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequestDto request)
    {
        var url = $"{_catalogApiBaseUrl}/products";
        return await _baseService.PostAsync<CreateProductRequestDto, ProductDto>(url, request);
    }    

    public async Task UpdateProductAsync(UpdateProductRequestDto request)
    {
        var url = $"{_catalogApiBaseUrl}/products";
        await _baseService.PutAsync<UpdateProductRequestDto>(url, request);
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var url = $"{_catalogApiBaseUrl}/products/{id}";
        await _baseService.DeleteAsync(url);
    }
}
