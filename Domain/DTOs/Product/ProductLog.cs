using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Product
{
    public class ProductLog
    {

       public int ProductId { get; set; }
       public int CategoryId { get; set; }
       public int SubCategoryId { get; set; }
       public DateTime RetrievedAt { get; set; }

    }
}
