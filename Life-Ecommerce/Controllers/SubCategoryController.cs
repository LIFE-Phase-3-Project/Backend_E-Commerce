using Application.Services.Subcategory;
using Application.Services.TokenService;
using Domain.DTOs.SubCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubCategoryController : ControllerBase
{
    private readonly ISubCategoryService _subCategoryService;
    private readonly TokenHelper _tokenHelper;

        public SubCategoryController(ISubCategoryService subCategoryService, TokenHelper token)
        {
            _subCategoryService = subCategoryService;
            _tokenHelper = token;
        }

        [HttpGet]
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
            var userRole = _tokenHelper.GetUserRole();
            if (userRole == null)
            {
                return Unauthorized("You are not logged in.");
            }
            else if (userRole != "Admin" && userRole != "SuperAdmin")
            {
                return Unauthorized("You are not authorized to perform this action.");
            }
            var createdSubCategory = await _subCategoryService.AddSubCategoryAsync(subCategoryDto);
                return CreatedAtAction(nameof(GetSubCategory), new { id = createdSubCategory.SubCategoryId }, createdSubCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubCategory(int id, UpdateSubCategoryDto subCategoryDto)
        {
            var userRole = _tokenHelper.GetUserRole();
            if (userRole == null)
            {
                return Unauthorized("You are not logged in.");
            }
            else if (userRole != "Admin" && userRole != "SuperAdmin")
            {
                return Unauthorized("You are not authorized to perform this action.");
            }
            await _subCategoryService.UpdateSubCategoryAsync(id, subCategoryDto);
                return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var userRole = _tokenHelper.GetUserRole();
            if (userRole == null)
            {
                return Unauthorized("You are not logged in.");
            }
            else if (userRole != "Admin" && userRole != "SuperAdmin")
            {
                return Unauthorized("You are not authorized to perform this action.");
            }
        var result = await _subCategoryService.DeleteSubCategoryAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
}