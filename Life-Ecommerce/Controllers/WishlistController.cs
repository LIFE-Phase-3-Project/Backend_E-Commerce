using Microsoft.AspNetCore.Mvc;
using Application.Services.Wishlist;
using Application.Services.TokenService;
namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly TokenHelper _tokenHelper;
        public WishlistController(IWishlistService wishlistService, TokenHelper tokenHelper)
        {
            _wishlistService = wishlistService;
            _tokenHelper = tokenHelper;
        }
        [HttpPost("AddWishlistEntry/{ProductId}")]
        public async Task<ActionResult> AddWishlistEntry(int ProductId)
        {
            var userId = _tokenHelper.GetUserId();
            if (userId == null)
            {
                return Unauthorized("You are not logged in.");
            }
            bool success = await _wishlistService.AddWishlistEntry(userId, ProductId);
            if (success) return Ok("Item added to wishlist successfully.");
            else return BadRequest("Item has already been added");
        }
        [HttpDelete("RemoveWishlistEntry/{ProductId}")]
        public async Task<ActionResult> RemoveWishlistEntry(int ProductId)
        {
            var userId = _tokenHelper.GetUserId();
            if (userId == null)
            {
                return Unauthorized("You are not logged in.");
            }
            await _wishlistService.RemoveWishlistEntry(userId, ProductId);
            return Ok("Item removed from wishlist successfully.");
        }
        [HttpGet("GetWishlistEntries")]
        public async Task<ActionResult> GetWishlistEntries()
        {
            var userId = _tokenHelper.GetUserId();
            if (userId == null)
            {
                return Unauthorized("You are not logged in.");
            }
            var entries = await _wishlistService.GetWishlistEntries(userId);
            if (entries == null) return BadRequest("Could not retrieve wishlist entries.");
            return Ok(entries);
        }
    }
}
