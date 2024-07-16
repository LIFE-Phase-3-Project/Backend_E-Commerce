using Domain.DTOs.Review;

namespace Application.Services.Review;

public interface IReviewService
{
    Task<CreateReviewDto> CreateReviewAsync(CreateReviewDto reviewDto, string token);
    // Task<CreateReviewDto> GetReviewByIdAsync(int id);
    Task<IEnumerable<ReadReviewDto>> GetReviewsByProductIdAsync(int productId);
    Task<bool> DeleteReviewAsync(int reviewId, string token);
}