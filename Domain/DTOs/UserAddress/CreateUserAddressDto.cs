using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.UserAddress
{
    public class CreateUserAddressDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string StreetAddress { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string City { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Country { get; set; }

        [Required]
        [Range(10000, 99999, ErrorMessage = "Postal code must be a 5-digit number.")]
        public short PostalCode { get; set; }
    }
}
