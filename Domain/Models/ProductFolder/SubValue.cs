using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class SubValue
    {
        [Key]
        public int SubValueId { get; set; }
        [Required]
        public string SubValueName { get; set; }
        [Required]
        public string Valuetype { get; set; }
        [Required]
        public int ValueId { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ValueId")]
        public Value Value { get; set; }
        public SubValue GetSubValue { get; set; }
    }
}
