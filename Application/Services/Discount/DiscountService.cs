using AutoMapper;
using Domain.DTOs.Discount;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Discount
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DiscountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Domain.Entities.Discount> ValidateDiscount(string code)
        {
            var discount = await _unitOfWork.Repository<Domain.Entities.Discount>().GetByCondition(x => x.Code == code && x.ExpiryDate > DateTime.Now).FirstOrDefaultAsync();
            if (discount == null)
            {
                return null;
            }
            return discount;
        }

        public async Task<IEnumerable<Domain.Entities.Discount>> GetDiscountsByUserId(string userId)
        {
            var discounts = await _unitOfWork.Repository<Domain.Entities.Discount>().GetAll().Where(x => x.UserId == userId && x.ExpiryDate > DateTime.Now).ToListAsync();
            return discounts;
        }


        public async Task CreateDiscount(CreateDiscountDto discount)
        {
            var discountToCreate = _mapper.Map<Domain.Entities.Discount>(discount);
             _unitOfWork.Repository<Domain.Entities.Discount>().Create(discountToCreate);
            await _unitOfWork.CompleteAsync();
            
        }

        public async Task UpdateDiscount(CreateDiscountDto discount)
        {
            var discountToUpdate = await _unitOfWork.Repository<Domain.Entities.Discount>().GetByCondition(x => x.Code == discount.Code).FirstOrDefaultAsync();
            if (discountToUpdate == null)
            {
                return;
            }
            discountToUpdate.Percentage = discount.Percentage;
            discountToUpdate.ExpiryDate = discount.ExpiryDate;
            discountToUpdate.Code = discount.Code;
            await _unitOfWork.CompleteAsync();
        }
    }
}
