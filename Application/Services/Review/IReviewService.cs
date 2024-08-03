using Domain.DTOs.Review;
using Domain.DTOs.Pagination;
namespace Application.Services.Review;

public interface IReviewService
{
    Task<CreateReviewDto> CreateReviewAsync(CreateReviewDto reviewDto, string token);
    // Task<CreateReviewDto> GetReviewByIdAsync(int id);
    Task<PaginatedInfo<ReviewDto>> GetReviewsByProductIdAsync(int productId, int page, int pageSize);
    Task<PaginatedInfo<ReviewDto>> GetReviewsByUserIdAsync(string userId, int page, int pageSize);

    Task<PaginatedInfo<ReviewDto>> GetAllReviews(int page, int pageSize);

    Task<bool> UpdateReviewAsync(int reviewId, UpdateReviewDto reviewDto, string token);

    Task<bool> DeleteReviewAsync(int reviewId, string token);
}