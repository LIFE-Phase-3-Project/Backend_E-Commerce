using Domain.DTOs.Product;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Wishlist
{
    public interface IWishlistService
    {
        Task<bool> AddWishlistEntry(string userId, int productId);
        Task RemoveWishlistEntry(string userId, int productId);
        Task<List<WishlistEntryDto>> GetWishlistEntries(string userId);
    }
}
