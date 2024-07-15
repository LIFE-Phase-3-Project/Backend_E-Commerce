using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    public int ProductId { get; set; }
    [JsonIgnore]
    public Product Product { get; set; }
}