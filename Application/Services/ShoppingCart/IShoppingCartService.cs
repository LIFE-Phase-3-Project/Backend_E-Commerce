using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.Product;
using Domain.DTOs.ShoppingCart;

namespace Application.Services.ShoppingCart
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto> GetCartContents(int? userId, string cardIdentifier);
        Task<bool> AddItem(int productId, int? userId, string cartIdentifier);
        Task<string> CreateCartForGuests();
        Task<bool> RemoveItem(int ProductId, int? userId, string cartIdentifier);
        Task<bool> UpdateItemQuantity(int ProductId, int Quantity, int? userId, string cartIdentifier);
        Task ClearCart(int? userId, string cartIdentifier);

    }
}
