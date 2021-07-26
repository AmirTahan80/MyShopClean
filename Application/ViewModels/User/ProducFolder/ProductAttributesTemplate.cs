using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class ProductAttributesTemplate
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Template { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Count { get; set; }
    }
}
