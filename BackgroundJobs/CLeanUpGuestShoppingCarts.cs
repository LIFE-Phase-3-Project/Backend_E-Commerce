using Application.BackgroundJobs.ShoppingCartCleanUp;

namespace BackgroundJobs
{
    public class CleanUpGuestShoppingCarts
    {
        private readonly IGuestShoppingCartService _guestShoppingCartService;

        public CleanUpGuestShoppingCarts(IGuestShoppingCartService guestShoppingCartService)
        {
            _guestShoppingCartService = guestShoppingCartService;
        }

        public async Task CleanUpAsync()
        {
            await _guestShoppingCartService.CleanUpAsync();
        }
    }
}
