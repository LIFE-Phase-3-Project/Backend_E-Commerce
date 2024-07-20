using Domain.DTOs.Product;
using Domain.Helpers;

namespace Application.Services.Product;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> GetProductByIdAsync(int id);
    Task AddProductAsync(CreateProductDto createProductDto);
    Task UpdateProductAsync(int id, CreateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(int id);
    Task<PaginatedInfo<ProductDto>> GetPaginatedProductsAsync(ProductFilterModel filters,int page, int pageSize);

    Task<PaginatedInfo<ProductDto>> GetProductsByCategoryIdAsync(int categoryId, int page, int pageSize);
    Task<PaginatedInfo<ProductDto>> GetProductsBySubCategoryIdAsync(int subCategoryId, int page, int pageSize);
    Task SoftDeleteProduct(int productId);
}