using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        [Required]
        public int TotalPrice { get; set; }
        public bool IsFinally { get; set; }

        //Navigation
        public ICollection<CartDetail> CartDetails { get; set; }
        [Required]
        public ApplicationUser User { get; set; }

    }
}