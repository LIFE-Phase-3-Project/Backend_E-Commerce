using Application.Services.ShoppingCart;
using Domain.DTOs.Product;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        public ShoppingCartController(IShoppingCartService shoppingCartService, IDataProtectionProvider dataProtectionProvider)
        {
            _shoppingCartService = shoppingCartService;
            _dataProtectionProvider = dataProtectionProvider;
        }
       
        private (int? userId, string cartIdentifier) GetUserOrCartIdentifier()
        {
           /* var userId = HttpContext.Items["UserId"] as string;
            string cartIdentifier = HttpContext.Session.GetString("CartIdentifier");

            if (int.TryParse(userId, out var intUserId))
            {
                return (intUserId, null); // Registered user
            }
            else
            {
                return (null, cartIdentifier); // Guest user or guest user without a cart
            }*/

            var userId = HttpContext.Items["UserId"] as string;
            string encryptedCartIdentifier;
 

            if (int.TryParse(userId, out var intUserId))
            {
                return (intUserId, null); 
            }
            

            else if (HttpContext.Request.Cookies.TryGetValue("CartIdentifier", out encryptedCartIdentifier))
            {
                var unprotectedCartIdentifier = _dataProtectionProvider.CreateProtector("CartIdentifierProtector").Unprotect(encryptedCartIdentifier);
                return (null, unprotectedCartIdentifier); 
            } else
            {
                return (null, null);
            }
        }
        [HttpPost("AddItemToCart/{ProductId}")]
        public async Task<ActionResult> AddItem(int ProductId)
        {
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();
            if (userId.HasValue)
            {
                var success = await _shoppingCartService.AddItem(ProductId, userId, null);
                if (success) return Ok("Item added successfully.");
                else return BadRequest("Could not add item to cart.");
            }
            else
            {
                if (cartIdentifier == null)
                {
                    // For unregistered users without a cart
                    cartIdentifier = await _shoppingCartService.CreateCartForGuests();
                    // Create a persistent cookie
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7), // Expires after 30 days
                        HttpOnly = true, // Enhance security by making the cookie accessible only through the HTTP protocol
                        IsEssential = true, 
                        Secure = true, // Ensures the cookie is sent only over HTTPS

                    };

                    var protectedCartIdentifier = _dataProtectionProvider.CreateProtector("CartIdentifierProtector").Protect(cartIdentifier);
                    HttpContext.Response.Cookies.Append("CartIdentifier", protectedCartIdentifier, cookieOptions);
                }
                var success = await _shoppingCartService.AddItem(ProductId, null, cartIdentifier);
                if (success) return Ok("Item added successfully.");
                else return BadRequest("Could not add item to cart.");
            }
        }
        [HttpDelete("DeleteItem/{ProductId}")] 
        public async Task<IActionResult> RemoveItem(int ProductId)
        {
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();
            if (userId.HasValue) {
                var result = await _shoppingCartService.RemoveItem(ProductId, userId, null);
                if (result) return Ok("Item removed successfully.");
                else return BadRequest("Could not remove item from cart.");
            }
            else
            {
                var result = await _shoppingCartService.RemoveItem(ProductId, null, cartIdentifier);
                if (result) return Ok("Item removed successfully.");
                else return BadRequest("Could not remove item from cart.");

            }

        }
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();

            if (userId != null)
            {
                var cart = await _shoppingCartService.GetCartContents(userId, null);
                return Ok(cart);
            } else if (cartIdentifier != null)
            {
                var cart = await _shoppingCartService.GetCartContents(null, cartIdentifier);
                return Ok(cart);
            } else return Ok("Cart is Empty"); 
        }

        [HttpPut("UpdateQuantity/{ProductId}/{Quantity}")]
        public async Task<IActionResult> UpdateItemQuantity(int ProductId, int Quantity)
        {
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();
            if (userId.HasValue) {
                var response = _shoppingCartService.UpdateItemQuantity(ProductId, Quantity, userId, null);
                return Ok("Item quantity updated successfully.");
            } else if (cartIdentifier != null)
            {
                var response = _shoppingCartService.UpdateItemQuantity(ProductId, Quantity, null, cartIdentifier);
                return Ok("Item quantity updated successfully.");
            } else return BadRequest("Could not update item quantity.");
        }
        [HttpDelete("ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();
            if (userId.HasValue) { 
                var response =  _shoppingCartService.ClearCart(userId, null);
                if (response.IsCompletedSuccessfully) return Ok("Cart cleared successfully.");
                else return BadRequest("Could not clear cart.");
            }
            else
            {
                var response =  _shoppingCartService.ClearCart(null, cartIdentifier);
                if (response.IsCompletedSuccessfully) return Ok("Cart cleared successfully.");
                else return BadRequest("Could not clear cart.");


            }
            

        }

    }
}
