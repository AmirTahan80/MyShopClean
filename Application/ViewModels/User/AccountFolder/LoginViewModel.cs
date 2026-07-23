using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="لطفاً {0} را وارد کنید.")]
        [Display(Name="پست الکترونیکی یا نام کاربری")]
        public string EmailOrName { get; set; }
        [Required(ErrorMessage ="لطفاً {0} را وارد کنید.")]
        [Display(Name="رمز عبور")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }
        public bool RememberMe { get; set; }
    }
}
