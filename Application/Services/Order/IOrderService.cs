using Domain.DTOs.Order;

namespace Application.Services.Order
{
    public interface IOrderService
    {

         Task<bool> CreateOrder(string token, OrderCreateDto orderDto);

        Task<IEnumerable<OrderWithDetailsDto>> GetAllOrders(); 

        Task<OrderWithDetailsDto> GetOrderById(int orderId); 

        Task<IEnumerable<OrderWithDetailsDto>> GetOrdersByUserId(string userId); 

        Task<bool> UpdateOrderStatus(int orderId, string newStatus);

        Task<List<MonthlyOrderDto>> GetOrdersPerMonth();

    }
}
