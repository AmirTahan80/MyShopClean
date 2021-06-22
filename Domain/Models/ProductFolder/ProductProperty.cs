using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ProductProperty
    {
        [Key]
        public int PropertyId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(250)]
        public string ValueType { get; set; }
        [Required]
        [MaxLength(250)]
        public string ValueName { get; set; }

        //Navigation
        public Product Product { get; set; }
    }
}
