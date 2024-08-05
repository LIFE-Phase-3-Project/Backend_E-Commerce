using Application.BackgroundJobs.ProductAnalytics;

namespace BackgroundJobs
{
    public class ProductAnalyticsJobs
    {
        private readonly IProductAnalyticsService _productAnalyticsService;

        public ProductAnalyticsJobs(IProductAnalyticsService productAnalyticsService)
        {
            _productAnalyticsService = productAnalyticsService;
        }

        public async Task RecalculateTopRatedProductsAsync()
        {
            await _productAnalyticsService.RecalculateTopRatedProductsAsync();
        }

        public async Task RecalculateTopSoldProductsAsync()
        {
            await _productAnalyticsService.RecalculateTopSoldProductsAsync();
        }
    }
}
