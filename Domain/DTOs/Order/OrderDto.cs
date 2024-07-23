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
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public short PostalCode { get; set; }
        public string Name { get; set; }
    }
}
