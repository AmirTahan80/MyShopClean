using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class UserFavorite
    {
        [Key]
        public int UserFavoriteId { get; set; }
        public int UserFavoritesDetailId { get; set; }
        public int ProductsCount { get; set; }
        public string UserId { get; set; }
        public DateTime CreateTime { get; set; }

        //Navigation
        public ICollection<UserFavoritesDetail> UserFavoritesDetails { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }




        //One To One With Product And AppliCationUser

        //One To One With UserFavoritesDetail
    }
}
