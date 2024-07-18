using System.Text.Json.Serialization;
using Domain.DTOs.User;

namespace Domain.DTOs.Review;

public class ReviewDto
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    public int ProductId { get; set; }
    // [JsonIgnore]
    // public int UserId { get; set; }
    public UserOverviewDto UserOverview { get; set; }
    
}