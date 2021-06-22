using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name="نام کاربر")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name="ایمیل")]
        [EmailAddress]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name="شماره مبایل")]
        public string UserPhone { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name="رمز عبور")]
        [DataType(DataType.Password)]
        public string UserPassWord { get; set; }
        [Required(ErrorMessage="این فیلد اجباری است")]
        [Display(Name="تکرار رمز عبور")]
        [Compare("UserPassWord")]
        [DataType(DataType.Password)]
        public string UserRePassword { get; set; }
    }
}