using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs.Product;

namespace Application.Services.ShoppingCart
{
    public interface IShoppingCartService
    {
        Task<Domain.Entities.ShoppingCart> GetCartContents(int userId);
        Task<bool> AddItem(int ProductId, int UserId);
        Task<bool> RemoveItem(int ProductId, int UserId);
        Task<bool> UpdateItemQuantity(int ProductId, int Quantity, int userId);
        Task ClearCart(int userId);

    }
}
