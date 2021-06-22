using System;
using System.Collections.Generic;

namespace Application.ViewModels.User
{
    public class GetCategoriesViewModel:GetAllCategoryViewModel
    {
        public ICollection<GetCategoryChildViewModel> Children { get; set; }
    }
}
