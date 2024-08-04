using Application.Services.ImageStorage;
using Application.Services.Product;
using Application.Services.ProductAnalytics;
using Domain.DTOs.Product;
using Microsoft.AspNetCore.Mvc;
using Presistence.Repositories.ProductAnalytics;

namespace Life_Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IStorageService _storageService;
    private readonly ILogger<ProductController> _logger;
    private readonly IProductAnalyticsService _productAnalyticsService;

    public ProductController(IProductService productService, IStorageService storageService, ILogger<ProductController> logger, IProductAnalyticsService productAnalyticsService)
    {
        _productService = productService;
        _logger = logger;
        _storageService = storageService;
        _productAnalyticsService = productAnalyticsService;

    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] ProductFilterModel filters, int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 50)
        {
            return BadRequest("Invalid pagination parameters.");
        }

        var paginatedProducts = await _productService.GetPaginatedProductsAsync(filters, page, pageSize);

        return Ok(paginatedProducts);
    }
    [HttpGet("testElastic")]
    public async Task<IActionResult> TestElasticsearchConnection()
    {
        var result = await _productService.TestElasticsearchConnectionAsync();
        return Ok(result);
    }

    [HttpGet("search-as-you-go")]
    public async Task<IEnumerable<ProductSearchDto>> SearchAsYouType(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return [];
        }
        var result = await _productService.SearchAsYouTypeAsync(query);

        return result;
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }
        _logger.LogInformation($"ProductId: {id}, CategoryId: {product.CategoryId}, SubcategoryId: {product.SubCategoryId} retrieved successfully.");
        return Ok(product);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> AddProduct([FromForm] CreateProductDto createProductDto)
    {
        await _productService.AddProductAsync(createProductDto);

        return Ok("Product added succesfully to the database and indexed to Elastic");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto updateProductDto)
    {

        var response = await _productService.UpdateProductAsync(id, updateProductDto);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategoryId(int categoryId, int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Invalid pagination parameters.");
        }

        var paginatedProducts = await _productService.GetProductsByCategoryIdAsync(categoryId, page, pageSize);

        if (paginatedProducts == null || !paginatedProducts.Items.Any())
        {
            return NotFound("No products found for the specified criteria.");
        }

        return Ok(paginatedProducts);
    }

    [HttpGet("subcategory/{subCategoryId}")]
    public async Task<IActionResult> GetProductsBySubCategoryId(int subCategoryId, int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Invalid pagination parameters.");
        }

        var paginatedProducts = await _productService.GetProductsBySubCategoryIdAsync(subCategoryId, page, pageSize);

        if (paginatedProducts == null || !paginatedProducts.Items.Any())
        {
            return NotFound("No products found for the specified criteria.");
        }

        return Ok(paginatedProducts);
    }

    [HttpDelete("softdelete/{productId}")]
    public async Task<ActionResult> SoftDeleteProduct(int productId)
    {
        await _productService.SoftDeleteProduct(productId);
        return NoContent();
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

    // add method to modify product discount
    [HttpPost("discount/{productId}")]
    public async Task<ActionResult> AddDiscountToProduct(int productId, decimal discount, DateTime ExpiryDate)
    {
        await _productService.AddDiscountToProduct(productId, discount, ExpiryDate);
        return Ok("Discount added successfully.");
    }
}

