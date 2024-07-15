using Domain.DTOs.Product;

namespace Application.Services.Product;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> GetProductByIdAsync(int id);
    Task AddProductAsync(CreateProductDto createProductDto);
    Task UpdateProductAsync(int id, CreateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(int id);
}