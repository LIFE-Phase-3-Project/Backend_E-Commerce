using Application;
using Application.Services.Email;
using Application.Services.Order;
using Application.Services.ShoppingCart;
using AutoMapper;
using Domain.DTOs.Order;
using Domain.DTOs.ShoppingCart;
using Moq;
using Presistence.Repositories.OrderRepo;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IShoppingCartService> _shoppingCartServiceMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _shoppingCartServiceMock = new Mock<IShoppingCartService>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _orderService = new OrderService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _shoppingCartServiceMock.Object,
                _orderRepositoryMock.Object,
                _emailServiceMock.Object
            );
        }

        [Fact]
        public async Task CreateOrder_ShouldReturnFalse_WhenCartIsEmpty()
        {
            // Arrange
            var token = CreateJwtToken("userId123");
            var orderDto = new OrderCreateDto { ShippingDate = DateTime.Now, UserAddressId = 1 };

            _shoppingCartServiceMock.Setup(s => s.GetCartContents(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ShoppingCartDto { Items = new List<ShoppingCartItemDto>() });

            // Act
            var result = await _orderService.CreateOrder(token, orderDto);

            // Assert
            Assert.False(result);
        }

        private string CreateJwtToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim> { new Claim("sub", userId) };
            var token = new JwtSecurityToken(claims: claims);
            return tokenHandler.WriteToken(token);
        }

       
    }
}
