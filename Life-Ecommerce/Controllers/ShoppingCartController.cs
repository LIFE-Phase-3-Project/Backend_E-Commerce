using Application.Services.ShoppingCart;
using Domain.DTOs.Product;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

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

        private (string userId, string cartIdentifier) GetUserOrCartIdentifier()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            string encryptedCartIdentifier;


            if (!(token == ""))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var userId = jwtToken.Claims.First(claim => claim.Type == "sub").Value; 
                return (userId, null); // Registered user
            }
            else if (HttpContext.Request.Cookies.TryGetValue("CartIdentifier", out encryptedCartIdentifier))
            {
                var unprotectedCartIdentifier = _dataProtectionProvider.CreateProtector("CartIdentifierProtector").Unprotect(encryptedCartIdentifier);
                return (null, unprotectedCartIdentifier); // Guest user with cart
            }
            else
            {
                return (null, null); // Guest user without cart
            }
        }

  


        [HttpDelete("DeleteItem/{ProductId}")]
        public async Task<IActionResult> RemoveItem(int ProductId)
        {
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();
            if (!string.IsNullOrEmpty(userId))
            {
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

            if (!string.IsNullOrEmpty(userId))
            {
                var cart = await _shoppingCartService.GetCartContents(userId, null);
                return Ok(cart);
            }
            else if (!string.IsNullOrEmpty(cartIdentifier))
            {
                var cart = await _shoppingCartService.GetCartContents(null, cartIdentifier);
                return Ok(cart);
            }
            else return Ok("Cart is empty");
        }

        [HttpPost("AddItemToCart/{ProductId}")]
        public async Task<ActionResult> AddItem(int ProductId)
        {
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();
            (bool success, string message) result;

            if (!string.IsNullOrEmpty(userId))
            {
                result = await _shoppingCartService.AddItem(ProductId, userId, null);
            }
            else
            {
                if (string.IsNullOrEmpty(cartIdentifier))
                {
                    cartIdentifier = await _shoppingCartService.CreateCartForGuests();
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7), 
                        HttpOnly = true, 
                        IsEssential = true,
                        Secure = false,
                    };

                    var protectedCartIdentifier = _dataProtectionProvider.CreateProtector("CartIdentifierProtector").Protect(cartIdentifier);
                    HttpContext.Response.Cookies.Append("CartIdentifier", protectedCartIdentifier, cookieOptions);
                }
                result = await _shoppingCartService.AddItem(ProductId, null, cartIdentifier);
            }

            if (result.success) return Ok(result.message);
            else return BadRequest(result.message);
        }

        [HttpPut("UpdateQuantity/{ProductId}/{Quantity}")]
        public async Task<IActionResult> UpdateItemQuantity(int ProductId, int Quantity)
        {
            if (Quantity <= 0) return BadRequest("Quantity must be greater than 0.");
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();
            (bool success, string message) result;

            if (!string.IsNullOrEmpty(userId))
            {
                result = await _shoppingCartService.UpdateItemQuantity(ProductId, Quantity, userId, null);
            }
            else if (!string.IsNullOrEmpty(cartIdentifier))
            {
                result = await _shoppingCartService.UpdateItemQuantity(ProductId, Quantity, null, cartIdentifier);
            }
            else
            {
                return BadRequest("Could not update item quantity.");
            }

            if (result.success) return Ok(result.message);
            else return BadRequest(result.message);
        }
        [HttpDelete("ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();
            if (!string.IsNullOrEmpty(userId))
            {
                var response = _shoppingCartService.ClearCart(userId, null);
                if (response.IsCompletedSuccessfully) return Ok("Cart cleared successfully.");
                else return BadRequest("Could not clear cart.");
            }
            else
            {
                var response = _shoppingCartService.ClearCart(null, cartIdentifier);
                if (response.IsCompletedSuccessfully) return Ok("Cart cleared successfully.");
                else return BadRequest("Could not clear cart.");
            }
        }

        [HttpPost("ApplyDiscount/{DiscountCode}")]
        public async Task<IActionResult> ApplyDiscount(string DiscountCode)
        {
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();
            if (!string.IsNullOrEmpty(userId))
            {
                var response = await _shoppingCartService.ApplyDiscount(userId, null, DiscountCode);
                if (response) return Ok("Discount applied successfully.");
                else return BadRequest("Could not apply discount.");
            }
            else
            {
                var response = await _shoppingCartService.ApplyDiscount(null, cartIdentifier, DiscountCode);
                if (response) return Ok("Discount applied successfully.");
                else return BadRequest("Could not apply discount.");
            }
        }

        [HttpDelete("RemoveDiscount/{DiscountCode}")]
        public async Task<IActionResult> RemoveDiscount(string DiscountCode)
        {
            var (userId, cartIdentifier) = GetUserOrCartIdentifier();
            if (!string.IsNullOrEmpty(userId))
            {
                var response = await _shoppingCartService.RemoveDiscount(userId, null, DiscountCode);
                if (response) return Ok("Discount removed successfully.");
                else return BadRequest("Could not remove discount.");
            }
            else
            {
                var response = await _shoppingCartService.RemoveDiscount(null, cartIdentifier, DiscountCode);
                if (response) return Ok("Discount removed successfully.");
                else return BadRequest("Could not remove discount.");
            }
        }

       
    }
}
