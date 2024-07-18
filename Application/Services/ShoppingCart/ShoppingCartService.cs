using AutoMapper;
using Domain.DTOs.Product;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Services.ShoppingCart
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ShoppingCartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddItem(int productId, int userId)
        {
            var cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                      .GetByCondition(c => c.UserId == userId)
                      .FirstOrDefaultAsync();
            if (cart == null)
            {
                var newCart = new Domain.Entities.ShoppingCart
                {
                    CartIdentifier = Guid.NewGuid().ToString(), // Generate a unique identifier
                    UserId = userId,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                };

                _unitOfWork.Repository<Domain.Entities.ShoppingCart>().Create(newCart);
                await _unitOfWork.CompleteAsync(); // Ensure the new cart is saved before querying it again

                cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                      .GetByCondition(c => c.UserId == userId)
                      .FirstOrDefaultAsync();
            }

            if (cart != null) 
            {
                var cartItem = new CartItem
                {
                    ProductId = productId,
                    ShoppingCartId = cart.Id,
                    Quantity = 1
                };
                var existingItem = await _unitOfWork.Repository<CartItem>().GetByCondition(c => c.ShoppingCartId == cart.Id && c.ProductId == productId).FirstOrDefaultAsync();

                if (existingItem != null) 
                {
                    existingItem.Quantity += 1;
                    await _unitOfWork.CompleteAsync();
                    return true;

                } 
                else
                {
                    _unitOfWork.Repository<CartItem>().Create(cartItem);
                    await _unitOfWork.CompleteAsync();
                    return true;
                }

            }

            return false; 
        }

        public Task ClearCart(int userId)
        {
            var yourCart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.UserId == userId).FirstOrDefault();
             if (yourCart == null) return Task.CompletedTask; // No cart found for the user (userId
             int cartId = yourCart.Id;
            var itemsToRemove = _unitOfWork.Repository<CartItem>().GetByCondition(c => c.ShoppingCartId == cartId).ToList();
            if (itemsToRemove != null)
            {
                foreach (var item in itemsToRemove)
                {
                    _unitOfWork.Repository<CartItem>().Delete(item);
                }
                _unitOfWork.Complete();
            }
            return Task.CompletedTask;
                
        }
        public async Task<Domain.Entities.ShoppingCart> GetCartContents(int userId)
        {
            var cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                        .GetByCondition(c => c.UserId == userId)
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                        .FirstOrDefaultAsync();

            // if (cart == null) return Enumerable.Empty<ShoppingCartItemDto>();

            var cartItemsDto = cart.CartItems.Select(ci => new ShoppingCartItemDto
            {
                ProductId = ci.ProductId,
                Title = ci.Product.Title, 
                Price = ci.Product.Price,
                Quantity = ci.Quantity
            }).ToList();

            return cart;
        }


        public Task<bool> RemoveItem(int ProductId, int userId)
        {
            var yourCart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.UserId == userId).FirstOrDefault();
            if (yourCart == null) return Task.FromResult(false); // No cart found for the user (userId
            int cartId = yourCart.Id;
            var itemToRemove = _unitOfWork.Repository<CartItem>().GetByCondition(c => c.ShoppingCartId ==cartId && c.ProductId == ProductId).FirstOrDefault();
            if (itemToRemove != null)
            {
                _unitOfWork.Repository<CartItem>().Delete(itemToRemove);
                _unitOfWork.Complete();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> UpdateItemQuantity(int ProductId, int Quantity, int userId)
        {
            var yourCart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.UserId == userId).FirstOrDefault();
            if (yourCart == null) return Task.FromResult(false); // No cart found for the user (userId
            int cartId = yourCart.Id;
            var itemToUpdate = _unitOfWork.Repository<CartItem>().GetByCondition(c => c.ShoppingCartId == cartId && c.ProductId == ProductId).FirstOrDefault();
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = Quantity;
                _unitOfWork.Complete();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
