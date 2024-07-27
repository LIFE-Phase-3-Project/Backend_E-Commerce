using Domain.DTOs.Order;
using Domain.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Order
{
    public interface IOrderService
    {

         Task<bool> CreateOrder(int? userId, string cartIdentifier, OrderDto orderDto);

        Task<IEnumerable<OrderDto>> GetAllOrders(); 

        Task<OrderDto> GetOrderById(int orderId); 

        Task<IEnumerable<OrderDto>> GetOrdersByUserId(int userId); 

        Task<bool> UpdateOrderStatus(int orderId, string newStatus);

        Task<List<MonthlyOrderDto>> GetOrdersPerMonth();

    }
}
