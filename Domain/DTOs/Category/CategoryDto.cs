using Domain.DTOs.SubCategory;

namespace Domain.DTOs.Category;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
        
    public List<SubCategoryDto> Subcategories { get; set; }
}