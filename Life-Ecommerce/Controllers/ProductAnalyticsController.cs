using Application.BackgroundJobs.ProductAnalytics;
using Microsoft.AspNetCore.Mvc;
using Domain.DTOs.Product;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAnalyticsController : ControllerBase
    {
        private readonly IProductAnalyticsService _productAnalyticsService;

        public ProductAnalyticsController(IProductAnalyticsService productAnalyticsService)
        {
            _productAnalyticsService = productAnalyticsService;
        }

        [HttpGet("top-rated-from-subCategory")]
        public async Task<ActionResult> GetTopRatedProductsBySubCategory(int subCategoryId)
        {
            var products = await _productAnalyticsService.GetTopRatedProductsBySubCategoryAsync(subCategoryId);
            return Ok(products);
        }

        [HttpGet("top-rated-from-Category")]
        public async Task<ActionResult> GetTopRatedProductsByCategory(int categoryId)
        {
            var products = await _productAnalyticsService.GetTopRatedProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("top-sold-from-subCategory")]
        public async Task<ActionResult> GetTopSoldProductsBySubCategory(int subCategoryId)
        {
            var products = await _productAnalyticsService.GetTopSoldProductsBySubCategoryAsync(subCategoryId);
            return Ok(products);
        }

        [HttpGet("top-sold-from-Category")]
        public async Task<ActionResult> GetTopSoldProductsByCategory(int categoryId)
        {
            var products = await _productAnalyticsService.GetTopSoldProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("top-viewed-from-subCategory")]
        public async Task<ActionResult> GetTopViewedProductsBySubCategory(int subCategoryId)
        {
            var products = await _productAnalyticsService.GetTopViewedProductsBySubCategoryAsync(subCategoryId);
            return Ok(products);
        }

        [HttpGet("top-viewed-from-Category")]
        public async Task<ActionResult> GetTopViewedProductsByCategory(int categoryId)
        {
            var products = await _productAnalyticsService.GetTopViewedProductsByCategoryAsync(categoryId);
            return Ok(products);
        }
    }
}
