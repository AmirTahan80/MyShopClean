using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ProductImages
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ImgSrc { get; set; }
        [Required]
        public string ImgFile { get; set; }
        [Required]
        public int ProductId { get; set; }

        // Navigation
        public Product Product { get; set; }
    }
}
