using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Review
{
    [Key]
    public int Id { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    public int ProductId { get; set; }
    public string UserId { get; set; }
    
    [JsonIgnore]
    public Product Product { get; set; }
    [JsonIgnore]
    public User User { get; set; }
}