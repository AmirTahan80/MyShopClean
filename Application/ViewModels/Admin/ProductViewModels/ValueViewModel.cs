using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class ValueViewModel
    {
        [Required]
        public int ValueId { get; set; }
        [Required]
        public string ValueName { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public int AdjectiveId { get; set; }

        //Navigation
        public AdjectiveViewModel Adjectives { get; set; }
        public SubValueViewModel SubValue { get; set; }
    }
}
