using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.DTOs.ShoppingCart
{
    public class ShoppingCartDto
    {

        public string CartIdentifier { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public List<ShoppingCartItemDto> Items { get; set; }
        public int TotalQuantity => Items.Sum(item => item.Quantity);
        // use discount to calculate total price just like in ShoppingCart.cs
        public decimal? DiscountPercentage { get; set; }
        public DateTime? DiscountExpiryDate { get; set; }

        public decimal TotalPrice
        {
            get
            {
                decimal total = Items.Sum(item => item.TotalPrice);
                if (DiscountPercentage.HasValue && DiscountExpiryDate > DateTime.Now)
                {
                    total *= (1 - DiscountPercentage.Value / 100);
                }
                return total;
            }
        }
    }
}
