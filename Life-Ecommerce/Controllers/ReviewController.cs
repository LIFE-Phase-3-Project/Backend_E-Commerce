using Application.Services.Review;
using Application.Services.TokenService;
using Domain.DTOs.Review;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly TokenHelper _tokenHelper;

    public ReviewController(IReviewService reviewService, TokenHelper tokenHelper )
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto reviewDto)
    {
        var userId = _tokenHelper.GetUserId();
        if (userId == null) {
            return Unauthorized("You are not logged in.");
        }
        var createdReview = await _reviewService.CreateReviewAsync(reviewDto, userId);
        return Ok(createdReview);
        
    }

    // [HttpGet("{id}")]
    // public async Task<IActionResult> GetReviewById(int id)
    // {
    //     var review = await _reviewService.GetReviewByIdAsync(id);
    //     if (review == null)
    //     {
    //         return NotFound();
    //     }
    //     return Ok(review);
    // }
    
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetReviewsByProductId(int productId, [FromQuery] int page = 1, [FromQuery] int pageSize=10)
    {
        var reviews = await _reviewService.GetReviewsByProductIdAsync(productId, page, pageSize);
        if (reviews == null)
        {
            return NotFound();
        }
        return Ok(reviews);
    }
    
    [HttpDelete("{reviewId}")]
    public async Task<IActionResult> DeleteReview(int reviewId)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        try
        {
            var result = await _reviewService.DeleteReviewAsync(reviewId, token);
            return Ok(new { Message = "Review deleted successfully", Success = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto reviewDto)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        try
        {
            var result = await _reviewService.UpdateReviewAsync(reviewId, reviewDto, token);
            return Ok(new { Message = "Review updated successfully", Success = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetReviewByUserId(string userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var reviews = await _reviewService.GetReviewsByUserIdAsync(userId, page, pageSize);
        if (reviews == null)
        {
            return NotFound();
        }
        return Ok(reviews);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllReviews([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var reviews = await _reviewService.GetAllReviews(page, pageSize);
        if (reviews == null)
        {
            return NotFound();
        }
        return Ok(reviews);
    }

}