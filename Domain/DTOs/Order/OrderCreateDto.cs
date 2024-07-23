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
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public short PostalCode { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Count { get; set; }
    }
}
