using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Domain.DTOs.Review;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

        reviewDto.UserId = int.Parse(userIdClaim.Value);

        var review = _mapper.Map<Domain.Entities.Review>(reviewDto);

        _unitOfWork.Repository<Domain.Entities.Review>().Create(review);
        await _unitOfWork.CompleteAsync();

        var createdReviewDto = _mapper.Map<CreateReviewDto>(review);
        return createdReviewDto;
    }
    
    public async Task<IEnumerable<ReadReviewDto>> GetReviewsByProductIdAsync(int productId)
    {
        var reviews = await _unitOfWork.Repository<Domain.Entities.Review>()
            .GetByCondition(r => r.ProductId == productId)
            .Include(r => r.User)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReadReviewDto>>(reviews);
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

        var userId = int.Parse(userIdClaim.Value);

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

        return true;
    }
    
    
}