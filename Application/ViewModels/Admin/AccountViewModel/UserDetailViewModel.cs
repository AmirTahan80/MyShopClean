using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class UserDetailViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="نام کاربری")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="ایمیل")]
        public string UserEmail { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="مقام")]
        public string RoleName { get; set; }
    }
}
