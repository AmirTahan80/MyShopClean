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
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImgSrc { get; set; }
        public int ProductPrice { get; set; }

        // Navigation
        public Product Product { get; set; }
        [ForeignKey("UserFavoriteId")]
        public UserFavorite UserFavorit { get; set; }
    }
}
