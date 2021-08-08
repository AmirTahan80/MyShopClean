using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class NewPassWordViewModel
    {
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="رمز عبور جاری")]
        [DataType(DataType.Password)]
        public string OldPassWord { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="رمز عبور جدید")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="تکرار رمز عبور جدید")]
        [DataType(DataType.Password)]
        public string ReNewPassword { get; set; }
    }
}
