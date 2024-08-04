using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.DTOs.User
{
    public class RegisterUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        [EmailAddress]
        public string Email { get; set; }
    }
}
