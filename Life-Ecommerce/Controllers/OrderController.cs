using Application.Services.Order;
using Application.Services.TokenService;
using Domain.DTOs.Order;
using Microsoft.AspNetCore.Mvc;

namespace Life_Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly TokenHelper _tokenHelper;

        public OrderController(IOrderService orderService, TokenHelper tokenHelper)
        {
            _orderService = orderService;
            _tokenHelper = tokenHelper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDto orderDto)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var result = await _orderService.CreateOrder(token, orderDto);


            if (result)
                return Ok("Order created successfully.");
            else
                return BadRequest("Failed to create order.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
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
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            var orders = await _orderService.GetOrdersByUserId(userId);
            return Ok(orders);
        }

      
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string newStatus)
        {
            var result = await _orderService.UpdateOrderStatus(id, newStatus);
            if (result)
                return Ok();

            return BadRequest("Failed to update order status");
        }

        [HttpGet("orders-per-month")]
        public async Task<IActionResult> GetOrdersPerMonth()
        {
            var monthlyOrders = await _orderService.GetOrdersPerMonth();
            return Ok(monthlyOrders);
        }
    }
}
