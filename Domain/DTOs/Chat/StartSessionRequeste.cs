using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Chat
{
    public class StartSessionRequest
    {
        public string CustomerEmail { get; set; }
        public string AdminEmail { get; set; }
    }
}
