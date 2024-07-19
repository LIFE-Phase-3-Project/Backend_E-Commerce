using Microsoft.AspNetCore.Mvc;
using Application.Services.Wishlist;
namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        [HttpPost("AddWishlistEntry/{ProductId}")]
        public async Task<ActionResult> AddWishlistEntry(int ProductId)
        {
            var userId = HttpContext.Items["UserId"] as string;
            if (userId == null)
            {
                return Unauthorized("You must be logged in to add items to your wishlist.");
            }
            bool success = await _wishlistService.AddWishlistEntry(int.Parse(userId), ProductId);
            if (success) return Ok("Item added to wishlist successfully.");
            else return BadRequest("Item has already been added");
        }
        [HttpDelete("RemoveWishlistEntry/{ProductId}")]
        public async Task<ActionResult> RemoveWishlistEntry(int ProductId)
        {
            var userId = HttpContext.Items["UserId"] as string;
            if (userId == null)
            {
                return Unauthorized("You must be logged in to remove items from your wishlist.");
            }
            await _wishlistService.RemoveWishlistEntry(int.Parse(userId), ProductId);
            return Ok("Item removed from wishlist successfully.");
        }
        [HttpGet("GetWishlistEntries")]
        public async Task<ActionResult> GetWishlistEntries()
        {
            var userId = HttpContext.Items["UserId"] as string;
            if (userId == null)
            {
                return Unauthorized("You must be logged in to view your wishlist.");
            }
            var entries = await _wishlistService.GetWishlistEntries(int.Parse(userId));
            if (entries == null) return BadRequest("Could not retrieve wishlist entries.");
            return Ok(entries);
        }
    }
}
