using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        
        public List<SubCategory> Subcategories { get; set; } // nje category ka shume subcategories
        public List<Product> Products { get; set; }
    }
}
