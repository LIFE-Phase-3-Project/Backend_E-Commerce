using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Repositories.OrderRepo
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int orderId);
    }
}
