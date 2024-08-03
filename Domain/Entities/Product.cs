using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    [ForeignKey("CategoryId")]
    public int CategoryId { get; set; }
    [ForeignKey("SubCategoryId")]
    public int SubCategoryId { get; set; }
    public string Color { get; set; }
    public List<string> Image { get; set; }
    public decimal Price { get; set; }
    public double Ratings { get; set; } = 0;
    public List<Review> Reviews { get; set; }
    public int Stock { get; set; }
    public bool IsDeleted { get; set; }
    public decimal? DiscountPercentage { get; set; } 
    public DateTime? DiscountExpiryDate { get; set; }
    [NotMapped]
    public decimal DiscountedPrice => DiscountPercentage.HasValue && DiscountExpiryDate > DateTime.Now ? Price - (Price * DiscountPercentage.Value / 100) : Price;
    public Category Category { get; set; } // Many to one - shumeProd to OneCat
    public SubCategory SubCategory { get; set; } // Many to one - shumeProd to OneSubcat
}