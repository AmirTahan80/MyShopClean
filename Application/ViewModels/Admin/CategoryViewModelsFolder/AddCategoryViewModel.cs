using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Admin
{
    public class AddCategoryViewModel
    {
        public int? ParentId { get; set; }

        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        public string Name { get; set; }
        [MaxLength(500)]
        public string Detail { get; set; }

        public IEnumerable<SelectListItem> CategoriesTreeView { get; set; }
    }
}
