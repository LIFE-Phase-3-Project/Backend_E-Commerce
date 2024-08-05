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
        Task<ShoppingCartDto> GetCartContents(string? userId, string cardIdentifier);
        Task<(bool success, string message)> AddItem(int productId, string? userId, string cartIdentifier);
        Task<string> CreateCartForGuests();
        Task<bool> RemoveItem(int ProductId, string? userId, string cartIdentifier);
        Task<(bool success, string message)> UpdateItemQuantity(int ProductId, int Quantity, string? userId, string cartIdentifier);
        Task ClearCart(string? userId, string cartIdentifier);
        Task MergeGuestCart(string cartIdentifier, string userId);

        Task<bool> ApplyDiscount(string? userId, string? cartIdentifier, string discountCode);
        Task<bool> RemoveDiscount(string? userId, string? cartIdentifier, string discountCode);

    }
}
