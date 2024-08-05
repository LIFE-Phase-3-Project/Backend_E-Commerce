using Domain.Entities;
using Elastic.CommonSchema;
using Microsoft.EntityFrameworkCore;
using Presistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Repositories.OrderRepo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly APIDbContext _context;

        public OrderRepository(APIDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderDetails.Any(od => od.UserId == userId))
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .IgnoreQueryFilters() // This will ignore the global filter for soft-deleted products
                .ToListAsync();

            return orders;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .IgnoreQueryFilters() // This will ignore the global filter for soft-deleted products
                .ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                       .ThenInclude(od => od.Product)
                       .IgnoreQueryFilters() // This will ignore the global filter for soft-deleted products
                       .FirstOrDefaultAsync(o => o.Id == orderId);
            return order;
        }
    }

}
