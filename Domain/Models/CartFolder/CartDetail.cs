using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Models
{
    public class CartDetail
    {
        [Key]
        public int CartDetailId { get; set; }
        [Required]
        public int ProductCount { get; set; }
        [Required]
        public int CartId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int TotalPrice { get; set; }
        [Required]
        public int ProductPrice { get; set; }

        public string AttributeValues { get; set; }

        //NavigationBar
        [Required]
        public Cart Cart { get; set; }
        [Required]
        public Product Product { get; set; }
        public AttributeTemplate Templates { get; set; }

    }
}