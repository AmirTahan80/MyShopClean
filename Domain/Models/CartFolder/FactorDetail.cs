using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class FactorDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public int ProductCount { get; set; }
        [Required]
        public int ProductPrice { get; set; }
        [Required]
        public int TotalPrice { get; set; }
        [Required]
        public string AttributesName { get; set; }
        [Required]
        public string AttributesValue { get; set; }
        [Required]
        public string ImageSrc { get; set; }

        public Factor Factor { get; set; }
    }
}
