using System.Collections;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Application.ViewModels.User
{
    public class GetProductForUserViewModel
    {
        #region Properties
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "نام محصول")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "تعداد")]
        public int Count { get; set; }
        [Required]
        [Display(Name = "قیمت")]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "عکس")]
        public string ImageSrc { get; set; }
        [Required]
        public string CatName{ get; set; }
        [Required]
        public int CatId { get; set; }
        #endregion
    }
}
