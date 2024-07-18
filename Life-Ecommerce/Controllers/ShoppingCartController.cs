using Application.Services.ShoppingCart;
using Domain.DTOs.Product;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }
        [HttpPost("AddItemToCart/{ProductId}")]
        public async Task<ActionResult> AddItem(int ProductId)
        {
            var userId = HttpContext.Items["UserId"] as string;
            var intUserId = int.Parse(userId);
            if (userId == null)
            {
                return Unauthorized("You are not authorized to view this content");
            }
            
            var response = await _shoppingCartService.AddItem(ProductId, intUserId);
            if (response)
            {
                return Ok("Item added successfully.");
            }
            else
            {
                return BadRequest("Could not add item to cart.");
            }
        }
        [HttpDelete("DeleteItem/{ProductId}")] 
        public async Task<IActionResult> RemoveItem(int ProductId)
        {
            var userId = HttpContext.Items["UserId"] as string;
            var intUserId = int.Parse(userId);
            var result = await _shoppingCartService.RemoveItem(ProductId, intUserId);
            if (result)
            {
                return Ok("Item removed successfully.");
            }
            else
            {
                return BadRequest("Could not remove item from cart.");
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
           var userId = int.Parse(HttpContext.Items["UserId"] as string);
           var cart = await _shoppingCartService.GetCartContents(userId);
           return Ok(cart);
        }
        [HttpPut("UpdateQuantity/{ProductId}/{Quantity}")]
        public async Task<IActionResult> UpdateItemQuantity(int ProductId, int Quantity)
        {
            var userId = int.Parse(HttpContext.Items["UserId"] as string);
            var response = await _shoppingCartService.UpdateItemQuantity(ProductId, Quantity, userId);
            if (response)
            {
                return Ok("Item quantity updated successfully.");
            }
            else
            {
                return BadRequest("Could not update item quantity.");
            }
        }
        [HttpDelete("ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = int.Parse(HttpContext.Items["UserId"] as string);
            var response =  _shoppingCartService.ClearCart(userId);
            if (response.IsCompletedSuccessfully)
            {
                return Ok("Cart cleared successfully.");
            }
            else
            {
                return BadRequest("Could not clear cart.");
            }
        }

    }
}
