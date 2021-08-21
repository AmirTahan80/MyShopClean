using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class GetProductProperties
    {
        [Required]
        public string ValueType { get; set; }
        [Required]
        public string ValueName { get; set; }
        [Required]
        public int ValueId { get; set; }
        public bool IsSelected { get; set; }
    }
}
