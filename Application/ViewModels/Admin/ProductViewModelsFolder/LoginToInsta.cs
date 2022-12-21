using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class LoginToInsta
    {
        [Required(ErrorMessage ="نام کاربری اجباری میباشد!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "رمز عبور اجباری میباشد!")]
        public string PassWord { get; set; }
    }
}
