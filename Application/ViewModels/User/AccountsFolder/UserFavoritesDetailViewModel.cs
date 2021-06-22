using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class UserFavoritesDetailViewModel
    {
        [Required]
        public int UserFavoritesDetailId { get; set; }
        [Required]
        public int UserFavoriteId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string ImgSrc { get; set; }
        [Required]
        public decimal ProductPrice { get; set; }

        [Required]
        public UserFavoriteViewModel UserFavoriteViewModel { get; set; }
    }
}
