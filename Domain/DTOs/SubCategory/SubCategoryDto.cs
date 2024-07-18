using Domain.DTOs.Category;
using Domain.DTOs.Product;

namespace Domain.DTOs.SubCategory;

public class SubCategoryDto
{
    public int SubCategoryId { get; set; }
    public string SubCategoryName { get; set; }
    public int CategoryId { get; set; }
    // public string CategoryName { get; set; }
    
    // public CategoryDto Category { get; set; }
}