using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class GetPrivacySettingViewModel
    {       
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="عنوان")]
        public string Title { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
    }
}
