using Application.Utilities.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class AddProductViewModel
    {
        [Required(ErrorMessage = "این فیلد اجباری است")]
        [Display(Name = "نام محصول")]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "این فیلد اجباری است")]
        [Display(Name = "توضیحات محصول")]
        public string Detail { get; set; }

        [Display(Name = "تعداد محصول")]
        public int Count { get; set; }

        [Display(Name = "قیمت محصول")]
        public int Price { get; set; }
        [Required(ErrorMessage = "این فیلد اجباری است")]
        [Display(Name = "دسته بندی")]
        public int CatId { get; set; }
        public List<string> ValueName { get; set; }
        public List<string> ValueType { get; set; }



        // NavigationBar

        [Required(ErrorMessage = "این فیلد اجباری است")]
        [Display(Name = "عکس محصول")]
        [ImageSize(5 * 1024 * 1024)]
        [ImageCount()]
        public IEnumerable<IFormFile> Images { get; set; }
        public IEnumerable<SelectListItem> CategoriesTreeView { get; set; }
    }
}
