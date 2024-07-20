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
using Domain.DTOs.ShoppingCart;

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

        public async Task<string> CreateCartForGuests()
        {
            var cartIdentifier = Guid.NewGuid().ToString();
            var cart = new Domain.Entities.ShoppingCart
            {
                CartIdentifier = cartIdentifier,
                UserId = null,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
            };
              _unitOfWork.Repository<Domain.Entities.ShoppingCart>().Create(cart);
            await _unitOfWork.CompleteAsync();
            return cartIdentifier;
        }
        public async Task<bool> AddItem(int productId, int? userId, string cartIdentifier)
        {
            var cart = new Domain.Entities.ShoppingCart();
            if (userId.HasValue)
            {
                 cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
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
            }
            else
            {
                cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                      .GetByCondition(c => c.CartIdentifier == cartIdentifier)
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

        public Task ClearCart(int? userId, string cartIdentifier)
        {
            var cart = new Domain.Entities.ShoppingCart();
            if (userId.HasValue)
            {
                cart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.UserId == userId).FirstOrDefault();
                if (cart == null) return Task.CompletedTask;
            } else
            {

                cart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.CartIdentifier == cartIdentifier).FirstOrDefault();
                if (cart == null) return Task.CompletedTask;
            }
            var itemsToRemove = _unitOfWork.Repository<CartItem>().GetByCondition(c => c.ShoppingCartId == cart.Id).ToList();
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
        public async Task<ShoppingCartDto> GetCartContents(int? userId, string cardIdentifier)
        {
            var cart = new Domain.Entities.ShoppingCart();
            if (userId != null)
            {
                 cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                    .GetByCondition(c => c.UserId == userId)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync();
            }
            else if (cardIdentifier != null) {
                 cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                    .GetByCondition(c => c.CartIdentifier == cardIdentifier)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync();
            } else
            {
                return null;
            }
            if (cart == null) return null;

            var cartItemsDto = cart.CartItems.Select(ci => new ShoppingCartItemDto
            {
                ProductId = ci.ProductId,
                Title = ci.Product.Title,
                Price = ci.Product.Price,
                Quantity = ci.Quantity
            }).ToList();

            var cartDto = new ShoppingCartDto
            {
                CartIdentifier = cart.CartIdentifier,
                DateCreated = cart.DateCreated,
                DateModified = cart.DateModified,
                Items = cartItemsDto
            };

            return cartDto;
        }



        public Task<bool> RemoveItem(int ProductId, int? userId, string cardIdentifier)
        {
            var cart = new Domain.Entities.ShoppingCart();
            if (userId != null)
            {
                cart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.UserId == userId).FirstOrDefault();
                if (cart == null) return Task.FromResult(false);
            }
            else if (cardIdentifier != null)
            {
                cart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.CartIdentifier == cardIdentifier).FirstOrDefault();
                if (cart == null) return Task.FromResult(false);
            }
            if (cart == null) return Task.FromResult(false); // No cart found for the user (userId
            int cartId = cart.Id;
            var itemToRemove = _unitOfWork.Repository<CartItem>().GetByCondition(c => c.ShoppingCartId ==cartId && c.ProductId == ProductId).FirstOrDefault();
            if (itemToRemove != null)
            {
                _unitOfWork.Repository<CartItem>().Delete(itemToRemove);
                _unitOfWork.Complete();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> UpdateItemQuantity(int ProductId, int Quantity, int? userId, string cardIdentifier)
        {
            var cart = new Domain.Entities.ShoppingCart();
            if (userId != null)
            {
                cart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.UserId == userId).FirstOrDefault();
                if (cart == null) return Task.FromResult(false);
            }
            else if (cardIdentifier != null)
            {
                cart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.CartIdentifier == cardIdentifier).FirstOrDefault();
                if (cart == null) return Task.FromResult(false);
            }
            int cartId = cart.Id;
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
