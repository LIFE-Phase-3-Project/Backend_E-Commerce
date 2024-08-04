using Domain.DTOs.Order;

namespace Application.Services.Order
{
    public interface IOrderService
    {

         Task<bool> CreateOrder(string token, OrderDto orderDto);

        Task<IEnumerable<OrderDto>> GetAllOrders(); 

        Task<OrderDto> GetOrderById(int orderId); 

        Task<IEnumerable<OrderDto>> GetOrdersByUserId(string userId); 

        Task<bool> UpdateOrderStatus(int orderId, string newStatus);

        Task<List<MonthlyOrderDto>> GetOrdersPerMonth();

    }
}
