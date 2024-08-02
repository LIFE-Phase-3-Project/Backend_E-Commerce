using Domain.DTOs.Product;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Repositories.ProductAnalytics
{

    public class ProductAnalyticsRepo : IProductAnalyticsRepo
    {
        private readonly APIDbContext _context;

        public ProductAnalyticsRepo(APIDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<TopProductDto>> CalculateTopRatedProductsbyCategory(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .OrderByDescending(p => p.Ratings)
                .Take(3)
                .Select(p => new TopProductDto
                {
                    Title = p.Title,
                    FirstImage = p.Image.FirstOrDefault(),
                    DiscountedPrice = p.DiscountedPrice
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TopProductDto>> CalculateTopRatedProductsbySubCategory(int subCategoryId)
        {
            return await _context.Products
                .Where(p => p.SubCategoryId == subCategoryId)
                .OrderByDescending(p => p.Ratings)
                .Take(3)
                .Select(p => new TopProductDto
                {
                    Title = p.Title,
                    FirstImage = p.Image.FirstOrDefault(),
                    DiscountedPrice = p.DiscountedPrice
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TopProductDto>> CalculateTopSoldProductsbyCategory(int categoryId)
        {
            return await _context.OrderDetails
                .Where(od => od.Product.CategoryId == categoryId)
                .GroupBy(od => od.Product)
                .OrderByDescending(g => g.Sum(od => od.Count))
                .Take(3)
                .Select(g => new TopProductDto
                {
                    Title = g.Key.Title,
                    FirstImage = g.Key.Image.FirstOrDefault(),
                    DiscountedPrice = g.Key.DiscountedPrice
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TopProductDto>> CalculateTopSoldProductsbySubCategory(int subCategoryId)
        {
            return await _context.OrderDetails
                .Where(od => od.Product.SubCategoryId == subCategoryId)
                .GroupBy(od => od.Product)
                .OrderByDescending(g => g.Sum(od => od.Count))
                .Take(3)
                .Select(g => new TopProductDto
                {
                    Title = g.Key.Title,
                    FirstImage = g.Key.Image.FirstOrDefault(),
                    DiscountedPrice = g.Key.DiscountedPrice
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TopProductDto>> CalculateTopViewedProductsbyCategory(int categoryId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TopProductDto>> CalculateTopViewedProductsbySubCategory(int SubCategoryId)
        {
            throw new NotImplementedException();
        }

    }
}
