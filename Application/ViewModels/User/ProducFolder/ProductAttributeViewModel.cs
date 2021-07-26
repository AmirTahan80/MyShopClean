using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class ProductAttributeViewModel
    {
        [Required]
        public string AttributesName { get; set; }
        [Required]
        public IEnumerable<SelectListItem> AttributesValue { get; set; }
    }
}
