using Application.Services.Category;
using Application.Services.Subcategory;
using Domain.DTOs.Product;
using Nest;
using Presistence.Repositories.ProductAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.ProductAnalytics
{


    public class ProductAnalyticsService : IProductAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductAnalyticsRepo _productAnalyticsRepo;
        private readonly ICategoryService _categoryService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IElasticClient _elasticsearchClient;

        public ProductAnalyticsService(IUnitOfWork unitOfWork, IProductAnalyticsRepo productAnalyticsRepo, ICategoryService categoryService, ISubCategoryService subCategoryService, IElasticClient elasticClient)
        {
            _unitOfWork = unitOfWork;
            _productAnalyticsRepo = productAnalyticsRepo;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
            _elasticsearchClient = elasticClient;
        }
        public Task<IEnumerable<TopProductDto>> GetTopRatedProductsByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TopProductDto>> GetTopRatedProductsBySubCategoryAsync(int subCategoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TopProductDto>> GetTopSoldProductsByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TopProductDto>> GetTopSoldProductsBySubCategoryAsync(int subCategoryId)
        {
            throw new NotImplementedException();
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
            var categories = await  _categoryService.GetAllCategoriesAsync();
            foreach (var category in categories)
            {
                var topRatedProducts = await _productAnalyticsRepo.CalculateTopRatedProductsbyCategory(category.CategoryId);
                // save top rated products to db
            }
            var subCategories = await _subCategoryService.GetAllSubCategoriesAsync();
            foreach (var subCategory in subCategories)
            {
                var topRatedProducts = await _productAnalyticsRepo.CalculateTopRatedProductsbySubCategory(subCategory.SubCategoryId);
                // save top rated products to db
            }

        }

        public async Task RecalculateTopSoldProductsAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            foreach (var category in categories)
            {
                var topRatedProducts = await _productAnalyticsRepo.CalculateTopSoldProductsbyCategory(category.CategoryId);
                // save top rated products to db
            }
            var subCategories = await _subCategoryService.GetAllSubCategoriesAsync();
            foreach (var subCategory in subCategories)
            {
                var topRatedProducts = await _productAnalyticsRepo.CalculateTopSoldProductsbySubCategory(subCategory.SubCategoryId);
                // save top rated products to db
            }
        }

        public async Task RecalculateTopViewedProductsAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            foreach (var category in categories)
            {
                var topRatedProducts = await _productAnalyticsRepo.CalculateTopViewedProductsbyCategory(category.CategoryId);
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
