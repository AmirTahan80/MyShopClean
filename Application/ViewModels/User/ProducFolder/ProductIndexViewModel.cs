using System.Collections.Generic;

namespace Application.ViewModels.User
{
    public class ProductIndexViewModel
    {
        public IEnumerable<GetListOfProductViewModel> Products { get; set; }
        public IEnumerable<GetCategoriesTreeViewViewModel> CategoriesTreeView { get; set; }
    }
}
