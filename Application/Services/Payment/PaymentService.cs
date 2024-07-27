using AutoMapper;
using Domain.DTOs.Payment;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void Create(Domain.Entities.Payment payment)
        {
            _unitOfWork.Repository<Domain.Entities.Payment>().Create(payment);
        }

        public async Task<Domain.Entities.Payment> GetPaymentById(int id)
        {
            return await _unitOfWork.Repository < Domain.Entities.Payment > ().GetByIdAsync(id);
        }

        public async Task<Domain.Entities.Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _unitOfWork.Repository<Domain.Entities.Payment>().GetByCondition(p => p.OrderId == orderId).FirstOrDefaultAsync();

        }

        public async Task<Domain.Entities.Payment?> GetPaymentByTransactionId(string id)
        {
            return await _unitOfWork.Repository<Domain.Entities.Payment>().GetByCondition(p => p.TransactionId == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Payment>> GetPayments()
        {
            return await _unitOfWork.Repository<Domain.Entities.Payment>().ToListAsync();
        }

        public async Task<List<MonthlyPaymentDto>> GetPaymentsPerMonth()
        {
            var payments = await _unitOfWork.Repository<Domain.Entities.Payment>().GetAll().ToListAsync();

            var monthlyCounts = payments
                .GroupBy(p => p.PaymentDate.ToString("yyyy-MM"))
                .Select(g => new MonthlyPaymentDto
                {
                    Month = g.Key,
                    Payments = g.Count()
                }).ToList();

            return monthlyCounts;
        }


        public async Task SaveChangesAsync()
        {
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Domain.Entities.Payment> CreateCashPayment(CreateCashPaymentDto createPaymentDto)
        {
            var order = await _unitOfWork.Repository<Domain.Entities.Order>().GetByIdAsync(createPaymentDto.OrderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            var payment = new Domain.Entities.Payment
            {
                PaymentDate = DateTime.UtcNow,
                Amount = order.OrderTotal, // Get amount from OrderTotal
                PaymentStatus = "Completed",
                PaymentMethod = "Cash",
                TransactionId = Guid.NewGuid().ToString(), // Generate a unique ID for cash transactions
                OrderId = createPaymentDto.OrderId
            };

            _unitOfWork.Repository<Domain.Entities.Payment>().Create(payment);
            await _unitOfWork.SaveChangesAsync();

            return payment;
        }


    }
}
