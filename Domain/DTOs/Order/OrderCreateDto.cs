using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Order
{
    public class OrderCreateDto
    {
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime ShippingDate { get; set; } = DateTime.Now.AddDays(7);
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public string Name { get; set; }
        public int UserAddressId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public int Count { get; set; }
    }
}
