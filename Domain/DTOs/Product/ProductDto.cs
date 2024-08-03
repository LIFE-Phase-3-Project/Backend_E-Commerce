using Domain.Entities;

namespace Domain.DTOs.Product;


public class ProductDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }
    public string Color { get; set; }
    public List<string> Image { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTime? DiscountExpiryDate { get; set; }
    public decimal DiscountedPrice => DiscountPercentage.HasValue && DiscountExpiryDate > DateTime.Now
        ? Price - (Price * DiscountPercentage.Value / 100)
        : Price;
    public double Ratings { get; set; }
    public int Stock { get; set; }
    public List<Entities.Review> Reviews { get; set; }
}