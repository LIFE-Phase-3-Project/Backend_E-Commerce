using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Order
{
    public class CreateOrderRequest
    {
        public int? UserId { get; set; }
        public string CartIdentifier { get; set; }
    }
}
