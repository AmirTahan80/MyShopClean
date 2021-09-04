using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class GetCategoriesTreeViewViewModel
    {
        public GetCategoriesTreeViewViewModel()
        {
            Children = new List<GetCategoriesTreeViewViewModel>();
        }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Count { get; set; }

        public int? ParentId { get; set; }

        public bool IsCategoryHasChild { get; set; } = false;


        //Navigation
        public ICollection<GetCategoriesTreeViewViewModel> Children { get; set; }

        public GetParentCategoryViewModel Parent { get; set; }
    }
}
