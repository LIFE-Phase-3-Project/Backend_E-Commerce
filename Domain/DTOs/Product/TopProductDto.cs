using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Product
{
    public class TopProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstImage { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int Ratings { get; set; }

    }
}
