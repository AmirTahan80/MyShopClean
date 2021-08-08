using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public DateTime CreateTime { get; set; }
        public int TotalPrice { get; set; }
        public bool IsFinally { get; set; }

        //Navigation
        public ICollection<CartDetail> CartDetails { get; set; }
        public ApplicationUser User { get; set; }

    }
}