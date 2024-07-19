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
    public int Ratings { get; set; }
    public List<Review> Reviews { get; set; }
    // se kam kuptu pse fronti ka kerku location per produkt
    // public Location Location { get; set; }
    public int Stock { get; set; }
    
    public Category Category { get; set; } // Many to one - shumeProd to OneCat
    public SubCategory SubCategory { get; set; } // Many to one - shumeProd to OneSubcat


}