using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class ForgotPassWordViewModel
    {
        [Required(ErrorMessage="فیلد {0} اجباری است")]
        [Display(Name="ایمیل")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }
    }
}