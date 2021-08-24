using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Factor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CartId { get; set; }
        [Required]
        public int TotalPrice { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UserAddress { get; set; }
        public string UserPhone { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserFamilly { get; set; }
        [Required]
        public FactorStatus Status { get; set; }
        [Required]
        public int RefId { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<FactorDetail> FactorDetails { get; set; }
        public ICollection<Discount> Discounts { get; set; }
    }
}
