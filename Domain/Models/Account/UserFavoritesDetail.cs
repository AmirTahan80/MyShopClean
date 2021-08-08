using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class UserFavoritesDetail
    {
        [Key]
        public int UserFavoritesDetailId { get; set; }
        [Required]
        public int UserFavoriteId { get; set; }
        [Required]
        public int ProductId { get; set; }

        // Navigation
        public Product Product { get; set; }
        [ForeignKey("UserFavoriteId")]
        public UserFavorite UserFavorit { get; set; }
    }
}
