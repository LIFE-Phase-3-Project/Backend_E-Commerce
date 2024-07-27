using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Order
{
    public class MonthlyOrderDto
    {
        public string Month { get; set; }
        public int Orders { get; set; } = 0;
    }
}
