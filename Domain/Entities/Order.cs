using System;
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
        public DateTime? PaymentDate { get; set; } 
        public decimal OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        [ForeignKey("UserAddressId")]
        public int UserAddressId { get; set; }
        public UserAddress UserAddress { get; set; }
        [ForeignKey("PaymentId")]
        public int? PaymentId { get; set; }

        public Payment Payment { get; set; }
    }
}
