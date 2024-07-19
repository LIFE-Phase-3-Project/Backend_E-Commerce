using Application.Services.Product;
using Domain.DTOs.Product;
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
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategoryId(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryIdAsync(categoryId);
        return Ok(products);
    }

    [HttpGet("subcategory/{subCategoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsBySubCategoryId(int subCategoryId)
    {
        var products = await _productService.GetProductsBySubCategoryIdAsync(subCategoryId);
        return Ok(products);
    }

    /*[HttpDelete("softdelete/{productId}")]
    public async Task<ActionResult> SoftDeleteProduct(int productId)
    {
        await _productService.SoftDeleteProduct(productId);
        return NoContent();
    }*/
}