using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.Admin
{
    public class DiscountViewMode
    {
        public int Id { get; set; }
        [Required]
        public string CodeName { get; set; }
        [Required]
        public int CodePrice { get; set; }

    }
}
