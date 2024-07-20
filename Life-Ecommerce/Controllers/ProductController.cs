using Application.Services.Product;
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

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] ProductFilterModel filters, int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Invalid pagination parameters.");
        }

        var paginatedProducts = await _productService.GetPaginatedProductsAsync(filters, page, pageSize);

        if (paginatedProducts == null || !paginatedProducts.Items.Any())
        {
            return NotFound("No products found for the specified criteria.");
        }

        return Ok(paginatedProducts);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
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
    public async Task<ActionResult> AddProduct([FromBody] CreateProductDto createProductDto)
    {
        await _productService.AddProductAsync(createProductDto);
        // return CreatedAtAction(nameof(GetProductById), new { id = createProductDto.Title }, createProductDto);
        return Ok("Product added succesfully");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] CreateProductDto updateProductDto)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        await _productService.UpdateProductAsync(id, updateProductDto);
        return NoContent();
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

