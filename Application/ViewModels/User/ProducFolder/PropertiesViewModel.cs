using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class PropertiesViewModel
    {
        [Required]
        public string PropertyName { get; set; }
        [Required]
        public string PropertyValue { get; set; }
    }
}
