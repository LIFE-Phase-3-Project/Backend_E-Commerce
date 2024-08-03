using Application.Services.Review;
using Domain.DTOs.Review;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto reviewDto)
    {
        // Extract the token from the request header
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var createdReview = await _reviewService.CreateReviewAsync(reviewDto, token);
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
    public async Task<IActionResult> GetReviewByUserId(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
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