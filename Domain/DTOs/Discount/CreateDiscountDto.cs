using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Discount
{
    public class CreateDiscountDto
    {
        public string? UserId { get; set; }
        public string Code { get; set; }
        public decimal Percentage { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}
