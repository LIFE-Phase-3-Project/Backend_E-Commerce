using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.ShoppingCart
{
    public class ShoppingCartItemDto
    {
        public int ProductId { get; set; }
           

        public string Title { get; set; }
        public List<string> Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }


        // Add these properties
        public decimal? DiscountPercentage { get; set; }
        public DateTime? DiscountExpiryDate { get; set; }

        public decimal TotalPrice
        {
            get
            {
                decimal total = Price * Quantity;
                if (DiscountPercentage.HasValue && DiscountExpiryDate > DateTime.Now)
                {
                    total *= (1 - DiscountPercentage.Value / 100);
                }
                return total;
            }
        }
    }

}
