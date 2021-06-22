using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Value
    {
        [Key]
        public int ValueId { get; set; }
        [Required]
        public string ValueName { get; set; }
        [Required]
        public int ValuePrice { get; set; }
        [Required]
        public int ValueCount { get; set; }        
        [Required]
        public int AdjectiveId { get; set; }


        //Navigation
        [ForeignKey("AdjectiveId")]
        public Adjective Adjective { get; set; }
        public ICollection<SubValue> SubValues { get; set; }
    }
}
