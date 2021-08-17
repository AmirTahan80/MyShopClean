﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int UserDetailId { get; set; }

        public UserFavorite UserFavorite { get; set; }
        [ForeignKey("UserDetailId")]
        public UserDetail UserDetail { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Cart> Carts { get; set; }
        public IEnumerable<Question> Questions { get; set; }

        //public ICollection<RequestForPay> RequestPaies { get; set; }
        //public ICollection<Factor> Factors { get; set; }
    }
}
