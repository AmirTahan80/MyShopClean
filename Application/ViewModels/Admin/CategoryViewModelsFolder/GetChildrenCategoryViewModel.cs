using System;
using System.Collections.Generic;

namespace Application.ViewModels.Admin
{
    public class GetChildrenCategoryViewModel
    {
        public int ChildId { get; set; }
        public string ChildName { get; set; }
        public int? ParentId { get; set; }


        public ICollection<GetChildrenCategoryViewModel> Children { get; set; }
    }
}
