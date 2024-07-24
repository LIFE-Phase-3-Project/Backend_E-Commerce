using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Payment
{
    public class MonthlyPaymentDto
    {
        public string Month { get; set; }
        public int Payments { get; set; }
    }
}
