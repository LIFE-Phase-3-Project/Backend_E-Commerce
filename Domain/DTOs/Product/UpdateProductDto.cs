using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Product
{
    public class UpdateProductDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int SubCategoryId { get; set; }
        public string Color { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "You can upload a maximum of 10 images.")]
        [FileTypeAndSize(new string[] { ".jpg", ".jpeg", ".png", ".gif" }, 20 * 1024 * 1024)]
        public List<IFormFile> Image { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public DateTime? DiscountExpiryDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
