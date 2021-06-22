using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class SubValueViewModel
    {
        [Required]
        public int SubValueId { get; set; }
        [Required]
        public string SubValueName { get; set; }
        [Required]
        public string Valuetype { get; set; }
        [Required]
        public int ValueId { get; set; }

        //Navigation
        public ValueViewModel Value { get; set; }
        public SubValueViewModel SubValue { get; set; }
    }
}
