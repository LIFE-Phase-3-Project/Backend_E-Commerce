using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Product
{
    public class UpdateProductDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int SubCategoryId { get; set; }
        public string Color { get; set; }
        public List<string> Image { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsDeleted { get; set; }
    }
}
