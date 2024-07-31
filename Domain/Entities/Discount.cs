using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Discount
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Code { get; set; }
        public decimal Percentage { get; set; }
        public DateTime CreatedAt   { get; set; }
        public DateTime UpdatedAt { get; set; }
        public  DateTime ExpiryDate { get; set; }
        public Boolean Active { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
