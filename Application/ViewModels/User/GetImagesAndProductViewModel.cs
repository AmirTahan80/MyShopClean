using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.ViewModels.User
{
    public class GetImagesAndProductViewModel:GetProductForUserViewModel
    {
        [Required]
        public string Detail { get; set; }
        [Required]
        public ICollection<GetImagesViewModel> Images { get; set; }
        [Required]
        public ICollection<SelectListItem> Comments { get; set; }
        [Required]
        public string OffCode { get; set; }

        public ICollection<GetProductForUserViewModel> Products { get; set; }
    }
}
