using Domain.DTOs.Discount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Discount
{
    public interface IDiscountService
    {
        Task<IEnumerable<Domain.Entities.Discount>> GetDiscountsByUserId(string userId);

        Task<Domain.Entities.Discount> ValidateDiscount(string code, string userId = "0");
        Task CreateDiscount(CreateDiscountDto discount);
        Task UpdateDiscount(CreateDiscountDto discount);
    }
}
