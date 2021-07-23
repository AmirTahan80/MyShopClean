using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class ProductAttribute
    {
        [Key]
        public int AttributeId { get; set; }
        [Required]
        [MaxLength(200)]
        public string AttributeName { get; set; }

        //Navigation
        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public ICollection<AttributeValue> AttributeValues { get; set; }
    }
}
