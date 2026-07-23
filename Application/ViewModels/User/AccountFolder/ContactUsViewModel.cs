using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessage = "لطفاً متن پیام را وارد کنید.")]
        [MaxLength(500, ErrorMessage = "متن پیام نمی‌تواند بیشتر از ۵۰۰ نویسه باشد.")]
        public string Text { get; set; }
        [Required(ErrorMessage = "لطفاً موضوع پیام را وارد کنید.")]
        [MaxLength(100, ErrorMessage = "موضوع پیام نمی‌تواند بیشتر از ۱۰۰ نویسه باشد.")]
        public string Topic { get; set; }
        [Required(ErrorMessage ="لطفاً نام خود را وارد کنید.")]
        [MaxLength(150,ErrorMessage ="نام نمی‌تواند بیشتر از ۱۵۰ نویسه باشد.")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="لطفاً نشانی ایمیل خود را وارد کنید.")]
        [EmailAddress(ErrorMessage = "نشانی ایمیل معتبر نیست.")]
        [MaxLength(150,ErrorMessage ="نشانی ایمیل نمی‌تواند بیشتر از ۱۵۰ نویسه باشد.")]
        public string Email { get; set; }
    }
}
