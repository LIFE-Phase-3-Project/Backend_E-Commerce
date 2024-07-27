using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Payment
{
    public class CreatePaymentDto
    {
        //[Required]
        //public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        [Required]
        public int OrderId { get; set; }
    }
}
