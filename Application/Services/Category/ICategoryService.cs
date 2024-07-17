using Domain.DTOs.Category;

namespace Application.Services.Category;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto> GetCategoryByIdAsync(int categoryId);
    Task AddCategoryAsync(CreateCategoryDto category);
    Task UpdateCategoryAsync(int id, CreateCategoryDto category);
    Task<bool> DeleteCategoryAsync(int categoryId);
}