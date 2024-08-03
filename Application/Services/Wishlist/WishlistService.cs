using Domain.DTOs.Product;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Wishlist
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        public WishlistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> AddWishlistEntry(string userId, int productId)
        {
            try
            {
                _unitOfWork.Repository<WishlistEntry>().Create(new WishlistEntry
                {
                    UserId = userId,
                    ProductId = productId
                });
                await _unitOfWork.CompleteAsync();
                return true; // Assuming the operation is successful
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return false;
                // In case of an exception, return false
            }
        }

        public async Task <List<WishlistEntryDto>> GetWishlistEntries(string userId)
        {
            var wishlistEntries =  _unitOfWork.Repository<WishlistEntry>().GetByCondition(we => we.UserId == userId);

            var wishlistEntryDtos = wishlistEntries.Select(entry => new WishlistEntryDto
            {
                ProductId = entry.ProductId,
                Name = entry.Product.Title,
                Price = entry.Product.Price,
                Image = entry.Product.Image.ToList()[0],
                // Map other required product properties as needed
            }).ToList();

            return wishlistEntryDtos;
        }

        public Task RemoveWishlistEntry(string userId, int productId)
        {
            var entry = new WishlistEntry
            {
                UserId = userId,
                ProductId = productId
            };
            try
            {
                _unitOfWork.Repository<WishlistEntry>().Delete(entry);
                return _unitOfWork.CompleteAsync();
            }
            catch (Exception) { 
                return  null;
            }
        }
    }
}
