using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class GetCategoriesTreeViewModel
    {
        [Required]
        public ICollection<GetCategoriesWithChildViewModel> CategoriesParent { get; set; }
    }
}