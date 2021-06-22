using System.Collections.Generic;
using System;
using Domain.Models;

namespace Domain.Models
{
    public class Cart
    {
        #region Properties
        public long CartId { get; set; }
        public string UserId { get; set; }
        public DateTime CreateTime { get; set; }
        public decimal Price { get; set; }
        public bool IsFinally { get; set; }
        #endregion
        #region Navigation
        public ICollection<CartDetail> CartDetails{get; set;}
        public ApplicationUser User{get; set;}
        #endregion

    }
}