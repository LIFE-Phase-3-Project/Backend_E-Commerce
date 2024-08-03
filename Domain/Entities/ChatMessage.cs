using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        [ForeignKey("SessionId")]
        public int SessionId { get; set; }
        public ChatSession ChatSession { get; set; }
    }

}
