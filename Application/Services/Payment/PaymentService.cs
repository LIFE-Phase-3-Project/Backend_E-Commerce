using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Domain.Entities.Payment> GetPaymentById(int id)
        {
            return await _unitOfWork.Repository < Domain.Entities.Payment > ().GetByIdAsync(id);
        }

        public async Task<Domain.Entities.Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _unitOfWork.Repository<Domain.Entities.Payment>().GetByCondition(p => p.OrderId == orderId).FirstOrDefaultAsync();

        }

        public Task<IEnumerable<Domain.Entities.Payment>> GetPayments()
        {
            throw new NotImplementedException();
        }
    }
}
