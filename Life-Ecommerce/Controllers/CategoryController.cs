using Application.Services.Category;
using Domain.DTOs.Category;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Life_Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /*[HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }*/

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();

        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(CreateCategoryDto categoryDto)
    {
        await _categoryService.AddCategoryAsync(categoryDto);
        return Ok("Category added succesfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(int id, CreateCategoryDto categoryDto)
    {
        if (categoryDto.CategoryName.IsNullOrEmpty())
        {
            return BadRequest("Name cannot be empty");
        }

        await _categoryService.UpdateCategoryAsync(id, categoryDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}