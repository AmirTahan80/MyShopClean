using System;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class OffCodeViewModel
    {
        [Required(ErrorMessage="فیلد {0} اجباری است")]
        [Display(Name="نام کد تخفیف")]
        public string CodeName { get; set; }
        [Required(ErrorMessage="فیلد {0} اجباری است")]
        [Display(Name="قیمت کد")]
        public decimal CodePrice { get; set; }
        [Required]
        public DateTime InsertTimeCode { get; set; }
        [Required(ErrorMessage="فیلد {0} اجباری است")]
        [Display(Name="تاریخ انقظا")]
        public DateTime ExpireTimeCode { get; set; }
    }
}