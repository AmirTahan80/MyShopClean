using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class GetProductsAndImageSrcViewModel
    {
        [Required]
        public int Id { get; set; }
        [Display(Name = "نام محصول")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "تعداد")]
        [Required]
        public int Count { get; set; }
        [Display(Name = "قیمت")]
        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }

        public bool IsSelected { get; set; }
        public bool InstagramPost { get; set; } = false;

        //NaviGations
        [Display(Name = "عکس")]
        [Required]
        public IEnumerable<string> ImageSrc { get; set; }
        public int ImagesCount { get; set; }
    }
}
