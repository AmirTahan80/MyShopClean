using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class SiteSettingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام سایت الزامی است.")]
        [MaxLength(150)]
        [Display(Name = "نام سایت")]
        public string SiteName { get; set; }

        [MaxLength(250)]
        [Display(Name = "شعار سایت")]
        public string SiteTagline { get; set; }

        [Required, RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "رنگ باید مانند #EF394E باشد.")]
        [Display(Name = "رنگ اصلی")]
        public string PrimaryColor { get; set; }

        [Required, RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "رنگ باید مانند #0EA5E9 باشد.")]
        [Display(Name = "رنگ دوم")]
        public string SecondaryColor { get; set; }

        [Required, RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "رنگ باید مانند #F59E0B باشد.")]
        [Display(Name = "رنگ تأکیدی")]
        public string AccentColor { get; set; }

        [MaxLength(250)]
        [Display(Name = "عنوان فوتر")]
        public string FooterTitle { get; set; }

        [MaxLength(2000)]
        [Display(Name = "توضیحات فوتر")]
        public string FooterDescription { get; set; }

        [MaxLength(500)]
        [Display(Name = "متن کپی‌رایت")]
        public string FooterCopyright { get; set; }

        [MaxLength(500)]
        [Display(Name = "آدرس")]
        public string Address { get; set; }

        [Phone, MaxLength(50)]
        [Display(Name = "شماره تماس")]
        public string Phone { get; set; }

        [EmailAddress, MaxLength(250)]
        [Display(Name = "ایمیل پشتیبانی")]
        public string SupportEmail { get; set; }

        [Url, MaxLength(500)]
        [Display(Name = "آدرس عمومی سایت")]
        public string PublicBaseUrl { get; set; }

        [Display(Name = "فعال‌سازی خروجی ترب")]
        public bool TorobEnabled { get; set; }

        [MinLength(32, ErrorMessage = "توکن دسترسی باید حداقل ۳۲ نویسه و غیرقابل حدس باشد.")]
        [MaxLength(200)]
        [Display(Name = "توکن دسترسی ترب")]
        public string TorobAccessToken { get; set; }

        [Display(Name = "فعال‌سازی خروجی ایمالز")]
        public bool EmallsEnabled { get; set; }

        [MinLength(32, ErrorMessage = "توکن دسترسی باید حداقل ۳۲ نویسه و غیرقابل حدس باشد.")]
        [MaxLength(200)]
        [Display(Name = "توکن دسترسی ایمالز")]
        public string EmallsAccessToken { get; set; }

        [Display(Name = "لوگو")]
        public IFormFile Logo { get; set; }

        public string LogoUrl { get; set; } = "";
    }
}
