using Domain.DTOs.Product;
using Elastic.CommonSchema;
using Microsoft.EntityFrameworkCore;
using Nest;
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
        private readonly IElasticClient _elasticClient;

        public ProductAnalyticsRepo(APIDbContext context, IElasticClient elasticClient)
        {
            _context = context;
            _elasticClient = elasticClient;
        }


        public async Task<IEnumerable<TopProductDto>> CalculateTopRatedProductsbyCategory(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .OrderByDescending(p => p.Ratings)
                .Take(3)
                .Select(p => new TopProductDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    FirstImage = p.Image[0],
                    DiscountedPrice = p.DiscountedPrice,
                    Ratings = p.Ratings
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
                    Id = p.Id,
                    Title = p.Title,
                    FirstImage = p.Image[0],
                    DiscountedPrice = p.DiscountedPrice,
                    Ratings = p.Ratings

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
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    FirstImage = g.Key.Image[0],
                    DiscountedPrice = g.Key.DiscountedPrice,
                    Ratings = g.Key.Ratings,
                    Count = g.Sum(od => od.Count)
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
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    FirstImage = g.Key.Image[0],
                    DiscountedPrice = g.Key.DiscountedPrice,
                    Ratings = g.Key.Ratings,
                    Count = g.Sum(od => od.Count)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TopProductDto>> CalculateTopViewedProductsbyCategory(int categoryId)
        {
            var searchResponse = await _elasticClient.SearchAsync<ProductLog>(s => s
               .Index("product_retrievals")
               .Size(0)
               .Query(q => q
                   .Term(t => t.CategoryId, categoryId)
               )
               .Aggregations(a => a
                   .Terms("top_products", t => t
                       .Field(f => f.ProductId)
                       .Size(3)
                       .Order(o => o.Descending("_count"))
                   )
               ));

            var topProductsLogs = searchResponse.Aggregations.Terms("top_products").Buckets
                .Select(b => new 
                {
                    ProductId = int.Parse(b.Key),
                    ViewCount = b.DocCount.GetValueOrDefault()
                })
                .ToList();

            var topProducts = new List<TopProductDto>();
            foreach (var topProduct in topProductsLogs)
            {
                var product = await _context.Products.FindAsync(topProduct.ProductId);
                if (product != null)
                {
                    topProducts.Add(new TopProductDto
                    {
                        Id = product.Id,
                        Title = product.Title,
                        FirstImage = product.Image[0],
                        DiscountedPrice = product.DiscountedPrice,
                        Ratings = product.Ratings,
                        Count = topProduct.ViewCount
                    });
                }
            }
            return topProducts;
        }

        public async Task<IEnumerable<TopProductDto>> CalculateTopViewedProductsbySubCategory(int subCategoryId)
        {
            var searchResponse = await _elasticClient.SearchAsync<ProductLog>(s => s
                .Index("product_retrievals")
                .Size(0)
                .Query(q => q
                    .Term(t => t.SubCategoryId, subCategoryId)
                )
                .Aggregations(a => a
                    .Terms("top_products", t => t
                        .Field(f => f.ProductId)
                        .Size(3)
                        .Order(o => o.Descending("_count"))
                    )
                ));

            var topProductsLogs = searchResponse.Aggregations.Terms("top_products").Buckets
                .Select(b => new
                {
                    ProductId = int.Parse(b.Key),
                    ViewCount = b.DocCount.GetValueOrDefault()
                })
                .ToList();

            var topProducts = new List<TopProductDto>();
            foreach (var topProduct in topProductsLogs)
            {
                var product = await _context.Products.FindAsync(topProduct.ProductId);
                if (product != null)
                {
                    topProducts.Add(new TopProductDto
                    {
                        Id = product.Id,
                        Title = product.Title,
                        FirstImage = product.Image[0],
                        DiscountedPrice = product.DiscountedPrice,
                        Ratings = product.Ratings,
                        Count = topProduct.ViewCount
                    });
                }
            }
            return topProducts;
        }

    }
}
