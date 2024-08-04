using System.Text.Json.Serialization;
using Domain.DTOs.User;

namespace Domain.DTOs.Review;

public class CreateReviewDto
{
    public string Comment { get; set; }
    public int Rating { get; set; }
    public int ProductId { get; set; }
    [JsonIgnore]
    public string UserId { get; set; }
    // [JsonIgnore]
    // public UserOverviewDto UserOverview { get; set; }
    
}