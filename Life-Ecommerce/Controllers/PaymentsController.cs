using Application.Services.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(int orderId)
        {
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            return Ok(payment);
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            var payment = await _paymentService.GetPaymentById(paymentId);
            return Ok(payment);
        }
    }
}
