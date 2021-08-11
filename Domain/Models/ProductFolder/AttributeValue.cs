using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class AttributeValue
    {
        [Key]
        public int ValueId { get; set; }
        [Required]
        [MaxLength(200)]
        public string ValueName { get; set; }
        [Required]
        public int ProductAttributeId { get; set; }

        //Navigation
        public ProductAttribute ProductAttribute { get; set; }
    }
}
