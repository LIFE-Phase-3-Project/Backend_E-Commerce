using Domain.DTOs.Pagination;
using Domain.DTOs.Product;

namespace Application.Services.Product;

public interface IProductService
{
    Task<ProductDto> GetProductByIdAsync(int id);
    Task AddProductAsync(CreateProductDto createProductDto);
    Task<bool> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(int id);
    Task<PaginatedInfo<ProductIndexDto>> GetPaginatedProductsAsync(ProductFilterModel filters,int page, int pageSize);
    Task<IEnumerable<ProductSearchDto>> SearchAsYouTypeAsync(string query);
    Task<PaginatedInfo<ProductDto>> GetProductsByCategoryIdAsync(int categoryId, int page, int pageSize);
    Task<PaginatedInfo<ProductDto>> GetProductsBySubCategoryIdAsync(int subCategoryId, int page, int pageSize);

    Task AddDiscountToProduct(int productId, decimal discount, DateTime ExpiryDate);
    Task SoftDeleteProduct(int productId);
    Task<bool> TestElasticsearchConnectionAsync();
}