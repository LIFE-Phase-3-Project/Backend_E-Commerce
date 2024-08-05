using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Order
{
    public class OrderWithDetailsDto
    {
        public DateTime ShippingDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal OrderTotal { get; set; }
        public int UserAddressId { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
    public class OrderDetailDto
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

}
