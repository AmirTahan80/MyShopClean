using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class GetProductFromCategoryViewModel
    {
        #region Properties
        [Required]
        public int CatId { get; set; }
        #endregion
        #region NavigationBar
        public ICollection<GetProductForUserViewModel> Products { get; set; }
        public ICollection<GetCategoryChildViewModel> CatChildren { get; set; }
        #endregion

    }
}
