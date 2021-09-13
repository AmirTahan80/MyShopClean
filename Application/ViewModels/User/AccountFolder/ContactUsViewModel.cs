using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessage = "فیلد متن نباید خالی باشد !")]
        [MaxLength(500, ErrorMessage = "نهایت تا 500 کاراکتر میتوانید وارد کنید !")]
        public string Text { get; set; }
        [Required(ErrorMessage = "فیلد موضوع نباید خالی باشد !")]
        [MaxLength(100, ErrorMessage = "نهایت تا 100 کاراکتر میتوانید وارد کنید !")]
        public string Topic { get; set; }
        [Required(ErrorMessage ="فیلد نام نمیتواند خالی باشد !")]
        [MaxLength(150,ErrorMessage ="نهایت 150 کاراکتر میتوانید وارد کنید !")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="قیلد ایمیل نمیتواند خالی باشد !")]
        [MaxLength(150,ErrorMessage ="نهایت تا 150 کاراکتر میتوانید وارد کنید !")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
