using Domain.Entities;

namespace Domain.DTOs.Product;


public class ProductDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int SubCategoryId { get; set; }
    public string Color { get; set; }
    public List<string> Image { get; set; }
    public decimal Price { get; set; }
    public int Ratings { get; set; }
    public int Stock { get; set; }
    public List<Review> Reviews { get; set; }
}