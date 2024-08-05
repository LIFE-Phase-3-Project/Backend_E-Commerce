using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.BackgroundJobs.ShoppingCartCleanUp
{
    public interface IGuestShoppingCartService
    {
        Task CleanUpAsync();
    }
}
