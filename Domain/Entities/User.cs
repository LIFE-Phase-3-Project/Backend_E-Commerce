﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName {  get; set; } 

        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string? Auth0UserId { get; set; }

        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public Role? UserRole { get; }
    }
}
