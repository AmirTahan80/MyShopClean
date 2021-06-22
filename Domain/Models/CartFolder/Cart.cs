using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Cart
    {
        public long CartId { get; set; }
        public string UserId { get; set; }
        public DateTime CreateTime { get; set; }
        public decimal Price { get; set; }
        public bool IsFinally { get; set; }

        //Navigation
        public ICollection<CartDetail> CartDetails { get; set; }
        public ApplicationUser User { get; set; }

    }
}