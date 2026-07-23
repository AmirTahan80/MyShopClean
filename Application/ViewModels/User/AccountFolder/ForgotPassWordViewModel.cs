using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class ForgotPassWordViewModel
    {
        [Required(ErrorMessage = "لطفاً {0} را وارد کنید.")]
        [EmailAddress(ErrorMessage = "نشانی ایمیل معتبر نیست.")]
        [Display(Name = "پست الکترونیک")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
