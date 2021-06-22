using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            UserDetail = new UserDetail();
        }
        public int UserDetailId { get; set; }

        public UserFavorite UserFavorite { get; set; }
        [ForeignKey("UserDetailId")]
        public UserDetail UserDetail { get; set; }


        //public ICollection<Comment> Comments { get; set; }
        //public ICollection<Cart> Carts { get; set; }
        //public ICollection<RequestForPay> RequestPaies { get; set; }
        //public ICollection<Factor> Factors { get; set; }
    }
}
