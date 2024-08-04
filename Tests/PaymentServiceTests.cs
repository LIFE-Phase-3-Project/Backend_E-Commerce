using System;
using System.Threading.Tasks;
using Application;
using Application.Services.Payment;
using AutoMapper;
using Domain.DTOs.Payment;
using Domain.Entities;
using Moq;
using Xunit;

namespace Tests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PaymentService _paymentService;

        public PaymentServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _paymentService = new PaymentService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateCashPayment_ShouldCreatePayment_WhenOrderExists()
        {
            // Arrange
            var orderId = 1;
            var orderTotal = 100.0m;
            var createPaymentDto = new CreateCashPaymentDto { OrderId = orderId };
            var order = new Order { Id = orderId, OrderTotal = orderTotal };
            var payment = new Payment { OrderId = orderId, PaymentDate = DateTime.UtcNow, PaymentId = 1 };

            _unitOfWorkMock.Setup(u => u.Repository<Order>().GetByIdAsync(orderId))
                .ReturnsAsync(order);

            _unitOfWorkMock.Setup(u => u.Repository<Payment>().Create(It.IsAny<Payment>()))
                .Callback<Payment>(p => p.PaymentId = 1); 

            // Act
            var result = await _paymentService.CreateCashPayment(createPaymentDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderTotal, result.Amount);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal("Completed", result.PaymentStatus);
            Assert.Equal("Cash", result.PaymentMethod);
            Assert.NotEmpty(result.TransactionId);

            _unitOfWorkMock.Verify(u => u.Repository<Order>().GetByIdAsync(orderId), Times.Once);
            _unitOfWorkMock.Verify(u => u.Repository<Payment>().Create(It.IsAny<Payment>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task CreateCashPayment_ShouldThrowException_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 1;
            var createPaymentDto = new CreateCashPaymentDto { OrderId = orderId };

            _unitOfWorkMock.Setup(u => u.Repository<Order>().GetByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _paymentService.CreateCashPayment(createPaymentDto));
            Assert.Equal("Order not found", exception.Message);

            _unitOfWorkMock.Verify(u => u.Repository<Order>().GetByIdAsync(orderId), Times.Once);
            _unitOfWorkMock.Verify(u => u.Repository<Payment>().Create(It.IsAny<Payment>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

      
    }
}
