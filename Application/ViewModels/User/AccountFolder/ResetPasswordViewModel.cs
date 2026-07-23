using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "پست الکترونیکی")]
        public string Email { get; set; }
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور جدید")]
        [MinLength(12, ErrorMessage = "رمز عبور جدید باید حداقل ۱۲ نویسه داشته باشد.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور جدید")]
        [Compare("NewPassword", ErrorMessage = "رمز عبور جدید و تکرار آن یکسان نیستند.")]
        public string ReNewPassword { get; set; }
    }
}
