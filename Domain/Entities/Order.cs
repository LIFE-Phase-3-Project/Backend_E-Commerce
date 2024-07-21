﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal OrderTotal { get; set; }
        public string OrderStatus { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public short PostalCode { get; set; }
        public string Name { get; set; }

    }
}
