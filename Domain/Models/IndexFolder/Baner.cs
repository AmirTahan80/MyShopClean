using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.IndexFolder
{
    public class Baner
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string Link { get; set; }
        [Required]
        public string Image { get; set; }

        public string BanerPlace { get; set; }
    }
}
