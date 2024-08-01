using Application.Services.ImageStorage;
using Application.Services.Product;
using AutoMapper;
using Domain.DTOs.Product;
using Domain.Entities;
using Domain.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IStorageService _storageService;
    private readonly ILogger<ProductController> _logger;
    public ProductController(IProductService productService, IStorageService storageService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
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
}

