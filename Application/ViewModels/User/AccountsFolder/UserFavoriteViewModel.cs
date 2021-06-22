using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class UserFavoriteViewModel
    {       
        [Required]
        public int UserFavoriteId{ get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public ICollection<UserFavoritesDetailViewModel> UserFavoritesDetailViewModel { get; set; }
    }
}
