using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class UserProfileDetailViewModel
    {
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name="نام کاربری")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name="ایمیل")]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name="تلفن")]
        public string UserPhone { get; set; }
        [Display(Name = "نام")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; } 
        [Display(Name="استان")]
        public string Province { get; set; } 
        [Display(Name="شهر")]
        public string City { get; set; }     
        [Display(Name="کشور")]
        public string Country { get; set; }  
        [Display(Name="منطقه")]
        public string Region { get; set; }   
        [Display(Name="آدرس دقیق")]
        public string Address { get; set; }

        //ProfileImage
        //Vip
    }
}