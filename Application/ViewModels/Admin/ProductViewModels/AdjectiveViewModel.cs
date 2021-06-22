using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class AdjectiveViewModel
    {
        [Required]
        public int AdjectiveId { get; set; }
        [Required]
        [MaxLength(250)]
        public string AdjectiveName { get; set; }

        //Navigation
        public IEnumerable<ValueViewModel> Values { get; set; }

    }
}
