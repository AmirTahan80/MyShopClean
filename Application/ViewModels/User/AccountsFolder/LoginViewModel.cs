using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "رمز عبور")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }
        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }
}