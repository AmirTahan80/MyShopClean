using System;

namespace Application.ViewModels.Admin
{
    public class CategoryParentViewModel
    {
        public string Name { get; set; } = "-";
        public int? ParentId { get; set; }
    }
}
