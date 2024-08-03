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
        public string? UserId { get; set; }
        public string Code { get; set; }
        public decimal Percentage { get; set; }
        public DateTime CreatedAt   { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public  DateTime ExpiryDate { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
