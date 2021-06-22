using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class IndexViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string ImgSrc { get; set; }
        [Required]
        public string ImgText { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Link { get; set; }
    }
}
