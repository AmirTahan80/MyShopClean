using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class GetAllCategoriesViewModel
    {
        public GetAllCategoriesViewModel()
        {
            Parent = new CategoryParentViewModel();
        }

        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        public string Name { get; set; }
        public string Detail { get; set; }
        public bool HasChild { get; set; }
        public bool IsSelected { get; set; }


        //Navigation
        public CategoryParentViewModel Parent { get; set; }
    }
}
