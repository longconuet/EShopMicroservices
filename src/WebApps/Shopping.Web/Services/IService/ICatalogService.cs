using Shopping.Web.Dtos;

namespace Shopping.Web.Services.IService;

public interface ICatalogService
{
    Task<List<ProductDto>> GetAllProductAsync();
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductRequestDto request);
    Task UpdateProductAsync(UpdateProductRequestDto request);
    Task DeleteProductAsync(Guid id);
}
