using Domain.DTOs.Pagination;
using Domain.DTOs.Product;
namespace Application.Services.Search
{
    public interface ISearchService
    {
        Task<bool> IndexProductAsync(ProductIndexDto product);
        Task<PaginatedInfo<ProductIndexDto>> SearchProductsAsync(ProductFilterModel filters, int page, int pageSize);
    }
}
    