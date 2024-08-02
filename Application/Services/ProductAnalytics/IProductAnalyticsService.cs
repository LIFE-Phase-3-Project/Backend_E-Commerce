using Domain.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ProductAnalytics
{
    public interface IProductAnalyticsService
    {
        Task RecalculateTopRatedProductsAsync();
        Task RecalculateTopSoldProductsAsync();
        Task RecalculateTopViewedProductsAsync();

        Task<IEnumerable<TopProductDto>> GetTopRatedProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<TopProductDto>> GetTopRatedProductsBySubCategoryAsync(int subCategoryId);
        Task<IEnumerable<TopProductDto>> GetTopSoldProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<TopProductDto>> GetTopSoldProductsBySubCategoryAsync(int subCategoryId);
        Task<IEnumerable<TopProductDto>> GetTopViewedProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<TopProductDto>> GetTopViewedProductsBySubCategoryAsync(int subCategoryId);
    }
}
