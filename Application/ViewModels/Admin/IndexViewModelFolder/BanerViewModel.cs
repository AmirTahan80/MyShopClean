using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.Admin
{
    public class BanerViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string Link { get; set; }

        public IFormFile Image { get; set; }
        public string ImagePath { get; set; }
        public string ImageLocation { get; set; }
    }
}
