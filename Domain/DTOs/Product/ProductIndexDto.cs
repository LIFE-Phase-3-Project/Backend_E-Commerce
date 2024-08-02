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

        public string FirstImage { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? DiscountExpiryDate { get; set; }
        public decimal DiscountedPrice => DiscountPercentage.HasValue && DiscountExpiryDate > DateTime.Now
            ? Price - (Price * DiscountPercentage.Value / 100)
            : Price;

        public int Stock { get; set; }
    }
}
