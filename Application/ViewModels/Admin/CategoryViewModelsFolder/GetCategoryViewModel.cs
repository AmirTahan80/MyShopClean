using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Application.ViewModels.Admin
{
    public class GetCategoryViewModel:GetAllCategoriesViewModel
    {
        public ICollection<GetChildrenCategoryViewModel> Children { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
