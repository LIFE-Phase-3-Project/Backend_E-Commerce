using Domain.DTOs.SubCategory;

namespace Application.Services.Subcategory;

public interface ISubCategoryService
{
    Task<IEnumerable<SubCategoryDto>> GetAllSubCategoriesAsync();
    Task<SubCategoryDto> GetSubCategoryByIdAsync(int subCategoryId);
    Task<SubCategoryDto> AddSubCategoryAsync(CreateSubCategoryDto subCategoryDto);
    Task UpdateSubCategoryAsync(int id, UpdateSubCategoryDto subCategoryDto);
    Task<bool> DeleteSubCategoryAsync(int subCategoryId);
}