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
        [MinLength(8)]
        public string UserPassWord { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است !")]
        [MinLength(8)]
        [Compare("UserPassWord", ErrorMessage = "تکرار گذرواژه با گذزواژه وارد شده یکسان نیست ! ")]
        public string UserRePassWord { get; set; }
        [Required(ErrorMessage ="مقام کاربر در وبسایت مهم است !!!")]
        public string RoleId { get; set; }

        public List<SelectListItem> RolesItem { get; set; }
    }
}
