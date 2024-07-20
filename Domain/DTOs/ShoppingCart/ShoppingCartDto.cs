using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
    }
}
