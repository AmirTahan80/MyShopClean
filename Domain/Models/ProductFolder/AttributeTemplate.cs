using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class AttributeTemplate
    {
        [Key]
        public int AttributeTemplateId { get; set; }
        [Required]
        [MaxLength(500)]
        public string Template { get; set; }
        public int AttrinbuteTemplatePrice { get; set; }
        public int AttrinbuteTemplateCount { get; set; }

        //Navigation

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}
