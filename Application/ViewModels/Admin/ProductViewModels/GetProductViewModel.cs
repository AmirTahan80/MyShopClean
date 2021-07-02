using Application.Utilities.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class GetProductViewModel
    {
        public int Id { get; set; }
        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = " فیلد {0}اجباری است! ")]
        public string Name { get; set; }
        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = " فیلد {0}اجباری است! ")]
        public string Detail { get; set; }
        [Display(Name = "قیمت")]
        [Required(ErrorMessage = " فیلد {0}اجباری است! ")]
        public int Price { get; set; }
        [Display(Name = "تعداد")]
        [Required(ErrorMessage = " فیلد {0}اجباری است! ")]
        public int Count { get; set; }
        [Display(Name = "نام دسته بندی")]
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public IList<string> Valuetype { get; set; }
        public IList<string> ValueName { get; set; }

        // NavigationBar
        [Display(Name = "عکس ها")]
        public IEnumerable<GetImagesViewModel> Images { get; set; }
        [ImageCount()]
        public IEnumerable<IFormFile> UploadImages { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<GetProductProperties> Properties { get; set; }

        public IEnumerable<int> DeletedProductImagesIds { get; set; }
        public IEnumerable<int> PropertiesDeletedIds { get; set; }
    }
}
