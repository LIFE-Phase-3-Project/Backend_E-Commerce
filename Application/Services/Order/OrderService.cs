using System.IdentityModel.Tokens.Jwt;
using Presistence.Repositories.OrderRepo;
using Application.Services.Email;
using Application.Services.ShoppingCart;
using AutoMapper;
using Domain.DTOs.Order;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IShoppingCartService shoppingCartService,
            IOrderRepository orderRepository,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _shoppingCartService = shoppingCartService;
            _orderRepository = orderRepository;
            _emailService = emailService;
        }


        public async Task<bool> CreateOrder(string token, OrderDto orderDto)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userId =int.Parse(jwtToken.Claims.First(claim => claim.Type == "nameid").Value);
            
            var cart = await _shoppingCartService.GetCartContents(userId, null);
            if (cart == null || !cart.Items.Any())
                return false;

            decimal orderTotal = cart.TotalPrice;

            var order = new Domain.Entities.Order
            {
                OrderDate = DateTime.Now,
                ShippingDate = orderDto.ShippingDate,
                PaymentDate = orderDto.PaymentDate,
                OrderTotal = orderTotal,
                OrderStatus = "Pending",
                UserAddressId = orderDto.UserAddressId,
                Name = orderDto.Name
            };

            _unitOfWork.Repository<Domain.Entities.Order>().Create(order);
            await _unitOfWork.CompleteAsync();

            if (cart.DiscountPercentage != null)
            {
                var discount = await _unitOfWork.Repository<Domain.Entities.Discount>()
                    .GetByCondition(d => d.UserId == userId && d.Code == "NewUserDiscount")
                    .FirstOrDefaultAsync();

                if (discount != null)
                {
                    _unitOfWork.Repository<Domain.Entities.Discount>().Delete(discount);
                    await _unitOfWork.CompleteAsync();
                }
            }
            foreach (var cartItem in cart.Items)
            {
                var product = await _unitOfWork.Repository<Domain.Entities.Product>()
                    .GetByIdAsync(cartItem.ProductId);

                if (product == null || product.Stock < cartItem.Quantity)
                {
                    return false;
                }

                var orderDetail = new Domain.Entities.OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Count = cartItem.Quantity,
                    Price = cartItem.TotalPrice,
                    UserId = userId
                };

                _unitOfWork.Repository<Domain.Entities.OrderDetail>().Create(orderDetail);

                if (product.Stock >= cartItem.Quantity)
                {
                    product.Stock -= cartItem.Quantity;
                    _unitOfWork.Repository<Domain.Entities.Product>().Update(product);
                }
            }

            await _unitOfWork.CompleteAsync();

            await _shoppingCartService.ClearCart(userId, null);

            var user = await _unitOfWork.Repository<Domain.Entities.User>()
                    .GetByIdAsync(userId);
            var subject = "Order Confirmation";
            var message = $"Dear {user.FirstName},\n\nThank you for your order! Your order ID is {order.Id}. We will notify you when your order status changes.\n\nBest regards,\nLIFE";

            await _emailService.SendEmailAsync(user.Email, subject, message);

            return true;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrders()
        {
            var orders = await _unitOfWork.Repository<Domain.Entities.Order>()
                .GetAll()
                .ToListAsync();

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderById(int orderId)
        {
            var order = await _unitOfWork.Repository<Domain.Entities.Order>()
                .GetByIdAsync(orderId);

            if (order == null)
                return null;

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserId(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<List<MonthlyOrderDto>> GetOrdersPerMonth()
        {
             var orders = await _unitOfWork.Repository<Domain.Entities.Order>().GetAll().ToListAsync();

            var monthlyCounts = orders
                .GroupBy(o => o.OrderDate.ToString("yyyy-MM"))
                .Select(g => new MonthlyOrderDto
                {
                    Month = g.Key,
                    Orders = g.Count()
                }).ToList();

            return monthlyCounts;
        }

        public async Task<bool> UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = await _unitOfWork.Repository<Domain.Entities.Order>()
                .GetByIdAsync(orderId);

            if (order == null)
                return false;

            order.OrderStatus = newStatus;
            _unitOfWork.Repository<Domain.Entities.Order>().Update(order);
            await _unitOfWork.CompleteAsync();

            var orderDetail = await _unitOfWork.Repository<Domain.Entities.OrderDetail>()
                .GetAll() 
                .Where(od => od.OrderId == orderId)
                .FirstOrDefaultAsync();

            if (orderDetail != null)
            {
                var user = await _unitOfWork.Repository<Domain.Entities.User>()
                    .GetByIdAsync(orderDetail.UserId);

                if (user != null)
                {
                    var subject = "Order Status Update";
                    var message = $"Dear {user.FirstName},\n\nYour order with ID {orderId} has been updated to '{newStatus}'.\n\nBest regards,\nLife";

                    await _emailService.SendEmailAsync(user.Email, subject, message);
                }
            }

            return true;
        }
    }
}
