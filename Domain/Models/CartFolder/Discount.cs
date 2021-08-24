using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Discount
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CodeName { get; set; }
        [Required]
        public int DiscountPrice { get; set; }
        [Required]
        public DateTime InsertTime { get; set; }
        [Required]
        public DateTime ExpireTime { get; set; }

        //Navigation

        public ICollection<Cart> Carts { get; set; }
        public ICollection<Factor> Factors { get; set; }
    }
}
