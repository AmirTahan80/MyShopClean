using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "فیلد {0} اجباری است !")]
        [MaxLength(250)]
        [MinLength(3)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است !")]
        [MaxLength(350)]
        [MinLength(3)]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است !")]
        [MinLength(12, ErrorMessage = "رمز عبور باید حداقل ۱۲ نویسه داشته باشد.")]
        public string UserPassWord { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است !")]
        [MinLength(12, ErrorMessage = "رمز عبور باید حداقل ۱۲ نویسه داشته باشد.")]
        [Compare("UserPassWord", ErrorMessage = "رمز عبور و تکرار آن یکسان نیستند.")]
        public string UserRePassWord { get; set; }
        [Required(ErrorMessage ="لطفاً نقش کاربر را انتخاب کنید.")]
        public string RoleId { get; set; }

        public List<SelectListItem> RolesItem { get; set; }
    }
}
