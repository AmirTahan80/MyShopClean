using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;



namespace Application.ViewModels.User
{
    public class GetCategoriesWithChildViewModel
    {
        public GetCategoriesWithChildViewModel()
        {
            CategoriesChild = new List<GetCategoriesWithChildViewModel>();
        }
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public bool HasChild { get; set; }

        //Navigation
        public ICollection<GetCategoriesWithChildViewModel> CategoriesChild { get; set; }
    }
}
