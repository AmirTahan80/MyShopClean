using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class ForgotPassWordViewModel
    {
        [Required(ErrorMessage = "فیلذ {0} اجباری است")]
        [Display(Name = "پست الکترونیک")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
