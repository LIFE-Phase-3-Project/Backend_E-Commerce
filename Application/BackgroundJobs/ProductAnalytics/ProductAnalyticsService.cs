using Application.Services.Category;
using Application.Services.Subcategory;
using Domain.DTOs.Product;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Nest;
using Presistence;
using Presistence.Repositories.ProductAnalytics;
using StackExchange.Redis;

namespace Application.BackgroundJobs.ProductAnalytics
{


    public class ProductAnalyticsService : IProductAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly APIDbContext _context;
        private readonly IProductAnalyticsRepo _productAnalyticsRepo;
        private readonly ICategoryService _categoryService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IElasticClient _elasticsearchClient;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        public ProductAnalyticsService(IUnitOfWork unitOfWork, IProductAnalyticsRepo productAnalyticsRepo, ICategoryService categoryService, ISubCategoryService subCategoryService, IElasticClient elasticClient, APIDbContext aPIDbContext, IConnectionMultiplexer connectionMultiplexer)
        {
            _unitOfWork = unitOfWork;
            _productAnalyticsRepo = productAnalyticsRepo;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _elasticsearchClient = elasticClient;
            _context = aPIDbContext;
            _redis = connectionMultiplexer;
            _db = _redis.GetDatabase();
        }
        public async Task<IEnumerable<TopProductDto>> GetTopRatedProductsByCategoryAsync(int categoryId)
        {
            if (await _db.KeyExistsAsync($"topratedcategory:{categoryId}"))
            {
                var topRatedCategoryProducts = await _db.StringGetAsync($"topratedcategory:{categoryId}");
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<TopProductDto>>(topRatedCategoryProducts);
            }
            else
            {
                var topRatedCategoryProducts = await _productAnalyticsRepo.CalculateTopRatedProductsbyCategory(categoryId);
                await _db.StringSetAsync($"topratedcategory:{categoryId}", System.Text.Json.JsonSerializer.Serialize(topRatedCategoryProducts));
                return topRatedCategoryProducts;
            }
        }

        public async Task<IEnumerable<TopProductDto>> GetTopRatedProductsBySubCategoryAsync(int subCategoryId)
        {
            if (await _db.KeyExistsAsync($"topratedsubcategory:{subCategoryId}"))
            {
                var topRatedSubCategoryProducts = await _db.StringGetAsync($"topratedsubcategory:{subCategoryId}");
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<TopProductDto>>(topRatedSubCategoryProducts);
            }
            else
            {
                var topRatedSubCategoryProducts = await _productAnalyticsRepo.CalculateTopRatedProductsbySubCategory(subCategoryId);
                await _db.StringSetAsync($"topratedsubcategory:{subCategoryId}", System.Text.Json.JsonSerializer.Serialize(topRatedSubCategoryProducts));
                return topRatedSubCategoryProducts;
            }
        }

        public async Task<IEnumerable<TopProductDto>> GetTopSoldProductsByCategoryAsync(int categoryId)
        {
            if (await _db.KeyExistsAsync($"topsoldcategory:{categoryId}"))
            {
                var topSoldCategoryProducts = await _db.StringGetAsync($"topsoldcategory:{categoryId}");
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<TopProductDto>>(topSoldCategoryProducts);
            }
            else
            {
                var topSoldCategoryProducts = await _productAnalyticsRepo.CalculateTopSoldProductsbyCategory(categoryId);
                await _db.StringSetAsync($"topsoldcategory:{categoryId}", System.Text.Json.JsonSerializer.Serialize(topSoldCategoryProducts));
                return topSoldCategoryProducts;
            }

        }
        public async Task<IEnumerable<TopProductDto>> GetTopSoldProductsBySubCategoryAsync(int subCategoryId)
        {
            if (await _db.KeyExistsAsync($"topsoldsubcategory:{subCategoryId}"))
            {
                var topSoldSubCategoryProducts = await _db.StringGetAsync($"topsoldsubcategory:{subCategoryId}");
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<TopProductDto>>(topSoldSubCategoryProducts);
            }
            else
            {
                var topSoldSubCategoryProducts = await _productAnalyticsRepo.CalculateTopSoldProductsbySubCategory(subCategoryId);
                await _db.StringSetAsync($"topsoldsubcategory:{subCategoryId}", System.Text.Json.JsonSerializer.Serialize(topSoldSubCategoryProducts));
                return topSoldSubCategoryProducts;
            }
        }

        public Task<IEnumerable<TopProductDto>> GetTopViewedProductsByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TopProductDto>> GetTopViewedProductsBySubCategoryAsync(int subCategoryId)
        {
            throw new NotImplementedException();
        }

        public async Task RecalculateTopRatedProductsAsync()
        {
            IEnumerable<TopProductDto> topRatedCategoryProducts = null;
            IEnumerable<TopProductDto> topRatedSubCategoryProducts = null;
            var categories = await _categoryService.GetAllCategoriesAsync();
            foreach (var category in categories)
            {
                topRatedCategoryProducts = await _productAnalyticsRepo.CalculateTopRatedProductsbyCategory(category.CategoryId);
                await _db.StringSetAsync($"topratedcategory:{category.CategoryId}", System.Text.Json.JsonSerializer.Serialize(topRatedCategoryProducts), TimeSpan.FromHours(25));
            }
            var subCategories = await _subCategoryService.GetAllSubCategoriesAsync();
            foreach (var subCategory in subCategories)
            {
                topRatedSubCategoryProducts = await _productAnalyticsRepo.CalculateTopRatedProductsbySubCategory(subCategory.SubCategoryId);
                await _db.StringSetAsync($"topratedsubcategory:{subCategory.SubCategoryId}", System.Text.Json.JsonSerializer.Serialize(topRatedSubCategoryProducts), TimeSpan.FromHours(25));

            }

        }

        public async Task RecalculateTopSoldProductsAsync()
        {
            IEnumerable<TopProductDto> topSoldCategoryProducts = null;
            IEnumerable<TopProductDto> topSoldSubCategoryProducts = null;
            var categories = await _categoryService.GetAllCategoriesAsync();
            foreach (var category in categories)
            {
                topSoldCategoryProducts = await _productAnalyticsRepo.CalculateTopSoldProductsbyCategory(category.CategoryId);
                await _db.StringSetAsync($"topsoldcategory:{category.CategoryId}", System.Text.Json.JsonSerializer.Serialize(topSoldCategoryProducts), TimeSpan.FromHours(25));

            }
            var subCategories = await _subCategoryService.GetAllSubCategoriesAsync();
            foreach (var subCategory in subCategories)
            {
                topSoldSubCategoryProducts = await _productAnalyticsRepo.CalculateTopSoldProductsbySubCategory(subCategory.SubCategoryId);
                await _db.StringSetAsync($"topsoldsubcategory:{subCategory.SubCategoryId}", System.Text.Json.JsonSerializer.Serialize(topSoldSubCategoryProducts), TimeSpan.FromHours(25));
            }
        }



        public async Task RecalculateTopViewedProductsAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            foreach (var category in categories)
            {
                // var topRatedProducts = await _productAnalyticsRepo.CalculateTopViewedProductsbyCategory(category.CategoryId);
                // save top rated products to db
            }
            var subCategories = await _subCategoryService.GetAllSubCategoriesAsync();
            foreach (var subCategory in subCategories)
            {
                var topRatedProducts = await _productAnalyticsRepo.CalculateTopViewedProductsbySubCategory(subCategory.SubCategoryId);
                // save top rated products to db
            }
        }
    }
}
