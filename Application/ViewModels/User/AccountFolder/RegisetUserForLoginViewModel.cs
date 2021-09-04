using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class RegisetUserForLoginViewModel
    {
        [Required(ErrorMessage = "فیلد {0} اجباری است !")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است !")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است !")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string PassWord { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است !")]
        [DataType(DataType.Password)]
        [Compare("PassWord", ErrorMessage = "رمز عبور با تکرار آن یکی نیست !!!")]
        [Display(Name = "تکرار رمز عبور")]
        public string RePassWord { get; set; }
    }
}
