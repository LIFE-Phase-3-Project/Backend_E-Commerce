using Domain.Entities;
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

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            // Query orders based on the userId via OrderDetail
            var orders = await _context.OrderDetails
                .Where(od => od.UserId == userId)
                .Select(od => od.OrderData)
                .Distinct()  // Ensure unique orders if necessary
                .ToListAsync();

            return orders;
        }
    }

}
