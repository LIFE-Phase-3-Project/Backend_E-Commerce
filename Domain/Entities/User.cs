using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.User
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FullName {  get; set; } 
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }

        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public Role? UserRole { get; }


    }
}
