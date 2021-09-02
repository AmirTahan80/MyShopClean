using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessage = "فیلد متن نباید خالی باشد !")]
        [MaxLength(500, ErrorMessage = "نهایتن تا 500 کاراکتر میتوانید بنویسید !")]
        public string Text { get; set; }
        [Required(ErrorMessage = "فیلد موضوع نباید خالی باشد !")]
        [MaxLength(100, ErrorMessage = "نهایتن تا 100 کاراکتر میتوانید بنویسید !")]
        public string Topic { get; set; }
    }
}
