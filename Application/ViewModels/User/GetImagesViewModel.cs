using System;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class GetImagesViewModel
    {
        [Required]
        public string ImagesSrc { get; set; }
    }
}
