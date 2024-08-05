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
using Google.Apis.Logging;
using Microsoft.Extensions.Logging;
using Application.Services.Discount;

namespace Application.Services.ShoppingCart
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDiscountService _discountService;
        // private readonly ILogger<ShoppingCartService> _logger;
        public ShoppingCartService(IUnitOfWork unitOfWork, IMapper mapper, IDiscountService discountService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _discountService = discountService;
        }

        public async Task MergeGuestCart(string cartIdentifier, string userId)
        {
            var guestCart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                .GetByCondition(c => c.CartIdentifier == cartIdentifier)
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync();

            if (guestCart == null) return;

            var userCart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                .GetByCondition(c => c.UserId == userId)
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync();

            if (userCart == null)
            {
                guestCart.UserId = userId;
                await _unitOfWork.CompleteAsync();
                return;
            }

            foreach (var item in guestCart.CartItems)
            {
                var existingItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                }
                else
                {
                    item.ShoppingCartId = userCart.Id;
                    userCart.CartItems.Add(item);
                }
            }

            _unitOfWork.Repository<Domain.Entities.ShoppingCart>().Delete(guestCart);
            await _unitOfWork.CompleteAsync();
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
        public async Task<(bool success, string message)> AddItem(int productId, string? userId, string cartIdentifier)
        {
            var cart = new Domain.Entities.ShoppingCart();
            if (userId != null)
            {
                cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                          .GetByCondition(c => c.UserId == userId)
                          .FirstOrDefaultAsync();
                if (cart == null)
                {
                    var newCart = new Domain.Entities.ShoppingCart
                    {
                        CartIdentifier = Guid.NewGuid().ToString(), 
                        UserId = userId,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                    };

                    _unitOfWork.Repository<Domain.Entities.ShoppingCart>().Create(newCart);
                    await _unitOfWork.CompleteAsync(); 

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
                var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetByIdAsync(productId);
                if (product == null || product.Stock < 1)
                {
                    // Product does not exist or not enough stock
                    return (false, $"Product is out of stock. Available stock: {product?.Stock ?? 0}");
                }

                var existingItem = await _unitOfWork.Repository<CartItem>()
                    .GetByCondition(c => c.ShoppingCartId == cart.Id && c.ProductId == productId)
                    .FirstOrDefaultAsync();

                if (existingItem != null)
                {
                    if (product.Stock < existingItem.Quantity + 1)
                    {
                        // Not enough stock to add more items
                        return (false, $"Not enough stock. You can add up to {product.Stock - existingItem.Quantity} more items.");
                    }

                    existingItem.Quantity += 1;
                    cart.DateModified = DateTime.Now;
                    await _unitOfWork.CompleteAsync();
                    return (true, "Item added successfully.");
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        ProductId = productId,
                        ShoppingCartId = cart.Id,
                        Quantity = 1
                    };

                    _unitOfWork.Repository<CartItem>().Create(cartItem);
                    cart.DateModified = DateTime.Now;
                    await _unitOfWork.CompleteAsync();
                    return (true, "Item added successfully.");
                }
            }

            return (false, "Could not add item to cart.");
        }


        public Task ClearCart(string? userId, string cartIdentifier)
        {
            var cart = new Domain.Entities.ShoppingCart();
            if (userId != null)
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
        public async Task<ShoppingCartDto> GetCartContents(string? userId, string cardIdentifier)
        {
            var cart = new Domain.Entities.ShoppingCart();
            if (userId != null)
            {
                 cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                    .GetByCondition(c => c.UserId == userId)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .Include(c => c.Discount)
                    .FirstOrDefaultAsync();
            }
            else if (cardIdentifier != null) {
                 cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                    .GetByCondition(c => c.CartIdentifier == cardIdentifier)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .Include(c => c.Discount)
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
                DiscountPercentage = ci.Product.DiscountPercentage,
                DiscountExpiryDate = ci.Product.DiscountExpiryDate,
                Quantity = ci.Quantity
            }).ToList();

            var cartDto = new ShoppingCartDto
            {
                CartIdentifier = cart.CartIdentifier,
                DateCreated = cart.DateCreated,
                DateModified = cart.DateModified,
                Items = cartItemsDto,
                DiscountPercentage = cart.Discount?.Percentage,
                DiscountExpiryDate = cart.Discount?.ExpiryDate
            };

            return cartDto;
        }

        public async Task<bool> ApplyDiscount(string? userId, string? cartIdentifier, string discountCode)
        {
            Domain.Entities.ShoppingCart? cart = new Domain.Entities.ShoppingCart();
            if (userId != null)
            {
                 cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                    .GetByCondition(c => c.UserId == userId)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync();
            }
            else if (cartIdentifier != null)
            {
                 cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                    .GetByCondition(c => c.CartIdentifier == cartIdentifier)
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync();
            }


            if (cart == null)
                return false;

            var discount = await _discountService.ValidateDiscount(discountCode);

            if (discount == null)
                return false;
            if (discount.UserId != null && discount.UserId != userId)
                return false;
            cart.DiscountId = discount.Id;
            _unitOfWork.Repository<Domain.Entities.ShoppingCart>().Update(cart);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> RemoveDiscount(string? userId, string? cartIdentifier, string discountCode)
        {
            Domain.Entities.ShoppingCart? cart = new Domain.Entities.ShoppingCart();
            if (userId != null)
            {
                cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                   .GetByCondition(c => c.UserId == userId)
                   .Include(c => c.CartItems)
                   .ThenInclude(ci => ci.Product)
                   .FirstOrDefaultAsync();
            }
            else if (cartIdentifier != null)
            {
                cart = await _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                   .GetByCondition(c => c.CartIdentifier == cartIdentifier)
                   .Include(c => c.CartItems)
                   .ThenInclude(ci => ci.Product)
                   .FirstOrDefaultAsync();
            }


            if (cart == null)
                return false;


            var discount = cart.DiscountId;
            if (discount == null)
                return false;

            cart.DiscountId = null;

            _unitOfWork.Repository<Domain.Entities.ShoppingCart>().Update(cart);
            await _unitOfWork.CompleteAsync();

            return true;
        }
        public Task<bool> RemoveItem(int ProductId, string? userId, string cardIdentifier)
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

        public async Task<(bool success, string message)> UpdateItemQuantity(int productId, int quantity, string? userId, string cartIdentifier)
        {
            var cart = new Domain.Entities.ShoppingCart();
            if (userId != null)
            {
                cart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.UserId == userId).FirstOrDefault();
                if (cart == null) return (false, "Cart not found.");
            }
            else if (cartIdentifier != null)
            {
                cart = _unitOfWork.Repository<Domain.Entities.ShoppingCart>().GetByCondition(c => c.CartIdentifier == cartIdentifier).FirstOrDefault();
                if (cart == null) return (false, "Cart not found.");
            }

            var product = await _unitOfWork.Repository<Domain.Entities.Product>().GetByIdAsync(productId);
            if (product == null || product.Stock < quantity)
            {
                // Product does not exist or not enough stock
                return (false, $"Not enough stock. Available stock: {product?.Stock ?? 0}");
            }

            int cartId = cart.Id;
            var itemToUpdate = _unitOfWork.Repository<CartItem>().GetByCondition(c => c.ShoppingCartId == cartId && c.ProductId == productId).FirstOrDefault();
            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
                cart.DateModified = DateTime.Now;
                _unitOfWork.Complete();
                return (true, "Item quantity updated successfully.");
            }
            return (false, "Item not found in cart.");
        }

    }
}
