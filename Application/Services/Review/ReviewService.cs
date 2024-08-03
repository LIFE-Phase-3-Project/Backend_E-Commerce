using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Domain.DTOs.Review;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Domain.DTOs.Pagination;
using Domain.DTOs.Product;
namespace Application.Services.Review;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CreateReviewDto> CreateReviewAsync(CreateReviewDto reviewDto, string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        if (jwtToken == null)
        {
            throw new SecurityTokenException("Invalid token");
        }

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nameid");
        if (userIdClaim == null)
        {
            throw new SecurityTokenException("UserId not found in token");
        }

        reviewDto.UserId = userIdClaim.Value;

        var review = _mapper.Map<Domain.Entities.Review>(reviewDto);

        _unitOfWork.Repository<Domain.Entities.Review>().Create(review);
        await _unitOfWork.CompleteAsync();

        await RecalculateProductRatingAsync(review.ProductId);

        var createdReviewDto = _mapper.Map<CreateReviewDto>(review);
        return createdReviewDto;
    }
    
    public async Task<bool> UpdateReviewAsync(int reviewId, UpdateReviewDto reviewDto, string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        if (jwtToken == null)
        {
            throw new SecurityTokenException("Invalid token");
        }

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nameid");
        if (userIdClaim == null)
        {
            throw new SecurityTokenException("UserId not found in token");
        }

        var userId = userIdClaim.Value;

        var review = await _unitOfWork.Repository<Domain.Entities.Review>()
            .GetByCondition(r => r.Id == reviewId)
            .FirstOrDefaultAsync();

        if (review == null)
        {
            throw new KeyNotFoundException("Review not found");
        }

        if (review.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this review");
        }

        review.Rating = reviewDto.Rating;
        review.Comment = reviewDto.Comment;

        _unitOfWork.Repository<Domain.Entities.Review>().Update(review);
        await _unitOfWork.CompleteAsync();

        await RecalculateProductRatingAsync(review.ProductId);

        return true;
    }
  
    public async Task<PaginatedInfo<ReviewDto>> GetReviewsByProductIdAsync(int productId, int page, int pageSize)
    {
        var query =  _unitOfWork.Repository<Domain.Entities.Review>()
            .GetByCondition(r => r.ProductId == productId)
            .Include(r => r.User);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var reviews = _mapper.Map<IEnumerable<ReviewDto>>(items);

        return new PaginatedInfo<ReviewDto>
        {
            Items = reviews.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PaginatedInfo<ReviewDto>> GetReviewsByUserIdAsync(string userId, int page, int pageSize)
    {
        var query =  _unitOfWork.Repository<Domain.Entities.Review>()
            .GetByCondition(r => r.UserId == userId)
            .Include(r => r.Product);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var reviews = _mapper.Map<IEnumerable<ReviewDto>>(items);

        return new PaginatedInfo<ReviewDto>
        {
            Items = reviews.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PaginatedInfo<ReviewDto>> GetAllReviews(int page = 1, int pageSize = 10)
    {
        var query =  _unitOfWork.Repository<Domain.Entities.Review>()
            .GetAll()
            .Include(r => r.Product)
            .Include(r => r.User);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var reviews = _mapper.Map<IEnumerable<ReviewDto>>(items);

        return new PaginatedInfo<ReviewDto>
        {
            Items = reviews.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    
    public async Task<bool> DeleteReviewAsync(int reviewId, string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        if (jwtToken == null)
        {
            throw new SecurityTokenException("Invalid token");
        }

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "nameid");
        if (userIdClaim == null)
        {
            throw new SecurityTokenException("UserId not found in token");
        }

        var userId = userIdClaim.Value;

        var review = await _unitOfWork.Repository<Domain.Entities.Review>()
            .GetByCondition(r => r.Id == reviewId)
            .FirstOrDefaultAsync();

        if (review == null)
        {
            throw new KeyNotFoundException("Review not found");
        }

        if (review.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this review");
        }

        _unitOfWork.Repository<Domain.Entities.Review>().Delete(review);
        await _unitOfWork.CompleteAsync();

        await RecalculateProductRatingAsync(review.ProductId);

        return true;
    }

    private async Task RecalculateProductRatingAsync(int productId)
    {
        var reviews = await _unitOfWork.Repository<Domain.Entities.Review>()
            .GetByCondition(r => r.ProductId == productId)
            .ToListAsync();

        if (reviews.Count == 0)
        {
            return;
        }

        var averageRating = reviews.Average(r => r.Rating);

        var product = await _unitOfWork.Repository<Domain.Entities.Product>()
            .GetByCondition(p => p.Id == productId)
            .FirstOrDefaultAsync();

        if (product != null)
        {
            product.Ratings = Math.Round(averageRating, 1);
            _unitOfWork.Repository<Domain.Entities.Product>().Update(product);
            await _unitOfWork.CompleteAsync();
        }
    }

}