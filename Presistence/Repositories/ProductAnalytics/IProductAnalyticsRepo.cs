using Domain.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Repositories.ProductAnalytics
{
    public interface IProductAnalyticsRepo
    {
        Task<IEnumerable<TopProductDto>> CalculateTopSoldProductsbyCategory(int categoryId);

        Task<IEnumerable<TopProductDto>> CalculateTopRatedProductsbyCategory(int categoryId);
        Task<IEnumerable<TopProductDto>> CalculateTopSoldProductsbySubCategory(int subCategoryId);

        Task<IEnumerable<TopProductDto>> CalculateTopRatedProductsbySubCategory(int subCategoryId);

        Task<IEnumerable<TopProductDto>> CalculateTopViewedProductsbyCategory(int categoryId);
        Task<IEnumerable<TopProductDto>> CalculateTopViewedProductsbySubCategory(int subCategoryId);


    }
}
