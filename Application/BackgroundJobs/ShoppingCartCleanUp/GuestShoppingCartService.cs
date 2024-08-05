using Microsoft.Extensions.Logging;
using Presistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.BackgroundJobs.ShoppingCartCleanUp
{
    public class GuestShoppingCartService : IGuestShoppingCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GuestShoppingCartService> _logger;


        public GuestShoppingCartService(IUnitOfWork unitOfWork, ILogger<GuestShoppingCartService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task CleanUpAsync()
        {
            var thresholdDate = DateTime.Now.AddMonths(-1);
            var oldGuestCarts = _unitOfWork.Repository<Domain.Entities.ShoppingCart>()
                .GetByCondition(c => c.UserId == null && c.DateModified < thresholdDate)
                .ToList();

            foreach (var cart in oldGuestCarts)
            {
                _unitOfWork.Repository<Domain.Entities.ShoppingCart>().Delete(cart);
            }

            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"{oldGuestCarts.Count} old guest shopping carts deleted.");
        }
    }
}
