using Application.Services.Discount;
using Domain.DTOs.Discount;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;
        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetDiscountsByUserId(int userId)
        {
            var discounts = await _discountService.GetDiscountsByUserId(userId);
            return Ok(discounts);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount(CreateDiscountDto discount)
        {
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
            await _discountService.UpdateDiscount(discount);
            return Ok(discount);
        }

    }
}
