using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class CallToUsViewModel
    {
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="نام کاربری")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="ایمیل")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="متن پیام")]
        public string Message { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "عنوان پیام")]
        public string Subject { get; set; }
    }
}
