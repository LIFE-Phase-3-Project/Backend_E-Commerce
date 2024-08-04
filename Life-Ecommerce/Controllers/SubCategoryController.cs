using Application.Services.Subcategory;
using Domain.DTOs.SubCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubCategoryController : ControllerBase
{
    private readonly ISubCategoryService _subCategoryService;

        public SubCategoryController(ISubCategoryService subCategoryService)
        {
            _subCategoryService = subCategoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<SubCategoryDto>>> GetSubCategories()
        {
            var subCategories = await _subCategoryService.GetAllSubCategoriesAsync();
            return Ok(subCategories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubCategoryDto>> GetSubCategory(int id)
        {
            var subCategory = await _subCategoryService.GetSubCategoryByIdAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return Ok(subCategory);
        }

        [HttpPost]
        public async Task<ActionResult<SubCategoryDto>> PostSubCategory(CreateSubCategoryDto subCategoryDto)
        {
            var createdSubCategory = await _subCategoryService.AddSubCategoryAsync(subCategoryDto);
            return CreatedAtAction(nameof(GetSubCategory), new { id = createdSubCategory.SubCategoryId }, createdSubCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubCategory(int id, UpdateSubCategoryDto subCategoryDto)
        {
            await _subCategoryService.UpdateSubCategoryAsync(id, subCategoryDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var result = await _subCategoryService.DeleteSubCategoryAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
}