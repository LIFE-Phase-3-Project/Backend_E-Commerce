using Application.Services.Discount;
using Application.Services.TokenService;
using Domain.DTOs.Discount;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        private readonly TokenHelper _tokenHelper;
        public DiscountController(IDiscountService discountService, TokenHelper tokenHelper)
        {
            _discountService = discountService;
            _tokenHelper = tokenHelper;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetDiscountsByUserId(string userId)
        {
            var discounts = await _discountService.GetDiscountsByUserId(userId);
            return Ok(discounts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount(CreateDiscountDto discount)
        {
            var userRole = _tokenHelper.GetUserRole();
            if (userRole == null)
            {
                return Unauthorized("You are not logged in.");
            }
            else if (userRole != "Admin" && userRole != "SuperAdmin")
            {
                return Unauthorized("You are not authorized to perform this action.");
            }
            await _discountService.CreateDiscount(discount);
            return Ok(discount);
        }

        [HttpPost("validate-discount")]
        public async Task<IActionResult> ValidateDiscount(string code)
        {
            var discount = await _discountService.ValidateDiscount(code);
            return Ok(discount);
        }

        [HttpPost("update-discount")]
        public async Task<IActionResult> UpdateDiscount(CreateDiscountDto discount)
        {
            var userRole = _tokenHelper.GetUserRole();
            if (userRole == null)
            {
                return Unauthorized("You are not logged in.");
            }
            else if (userRole != "Admin" && userRole != "SuperAdmin")
            {
                return Unauthorized("You are not authorized to perform this action.");
            }
            await _discountService.UpdateDiscount(discount);
            return Ok(discount);
        }

    }
}
