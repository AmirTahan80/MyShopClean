using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class EditPassWordViewModel
    {
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "رمز عبور فعلی")]
        [DataType(DataType.Password)]
        public string CurentPassWord { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "رمز عبور جدید")]
        [DataType(DataType.Password)]
        public string NewPassWord { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "تکرار رمز عبور جدید")]
        [DataType(DataType.Password)]
        [Compare("NewPassWord",ErrorMessage ="رمز عبور یکی نیست")]
        public string ConfirmPassWord { get; set; }
    }
}
