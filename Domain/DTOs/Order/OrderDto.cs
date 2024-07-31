using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Order
{
    public class OrderDto
    {
        public DateTime ShippingDate { get; set; }
        public DateTime PaymentDate { get; set; }

        public decimal OrderTotal { get; set; }
        public int UserAddressId { get; set; }
        public string Name { get; set; }
    }
}
