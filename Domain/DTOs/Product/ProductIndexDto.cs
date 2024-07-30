using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Product
{
    public class ProductIndexDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Color { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }
    }
}
