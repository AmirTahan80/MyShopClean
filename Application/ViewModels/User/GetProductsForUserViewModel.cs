using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class GetProductsForUserViewModel
    {
        [Required]
        public int CatId { get; set; }
        [Required]
        public string CatName { get; set; }

        public HomePageViewModel HomePage { get; set; }
        public IEnumerable<GetProductForUserViewModel> Products { get; set; }
        public ICollection<GetCategoryParentViewModel> Categories{get; set;}

    }
}
