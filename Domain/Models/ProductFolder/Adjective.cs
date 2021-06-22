using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Adjective
    {
        [Key]
        public int AdjectiveId { get; set; }
        [Required]
        public string AdjectiveName { get; set; }
        [Required]
        public int ProductId { get; set; }

        //Navigation

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public ICollection<Value>  Values { get; set; }
    }
}
