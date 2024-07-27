using Domain.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Payment
{
    public interface IPaymentService
    {

        Task<IEnumerable<Domain.Entities.Payment>> GetPayments();

        Task<Domain.Entities.Payment> GetPaymentById(int id);

        Task<Domain.Entities.Payment> GetPaymentByOrderIdAsync(int orderId);


        Task<List<MonthlyPaymentDto>> GetPaymentsPerMonth();
        Task<Domain.Entities.Payment?> GetPaymentByTransactionId(string id);

        void Create(Domain.Entities.Payment payment);
        Task SaveChangesAsync();

        Task<Domain.Entities.Payment> CreateCashPayment(CreateCashPaymentDto createPaymentDto);

    }
}
