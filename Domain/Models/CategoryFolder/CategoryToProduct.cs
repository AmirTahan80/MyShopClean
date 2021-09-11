using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class CategoryToProduct
    {
        [Key]
        public int CategoryId { get; set; }
        [Key]
        public int ProductId { get; set; }

        public Product Product { get; set; }
        public Category Category { get; set; }
    }
}
