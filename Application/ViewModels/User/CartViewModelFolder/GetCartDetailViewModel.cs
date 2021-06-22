using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class GetCartDetailViewModel
    {
        [Required]
        public int CartId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int CartDetailId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public decimal ProductPrice { get; set; }
        [Required]
        public int ProductCount { get; set; }
        [Required]
        public string ImgSrc { get; set; }
    }
}