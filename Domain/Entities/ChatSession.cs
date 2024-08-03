using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatSession
    {
        public int Id { get; set; }
        public string CustomerEmail { get; set; }
        public string AdminEmail { get; set; }

        public string Status { get; set; } = "pending";
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        
    }

}
