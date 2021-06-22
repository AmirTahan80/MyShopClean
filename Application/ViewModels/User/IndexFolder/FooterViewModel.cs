using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class FooterViewModel
    {
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "")]
        public string Title { get; set; }

        public ICollection<FooterSubjectViewModel> Subjects { get; set; }
    }
}
