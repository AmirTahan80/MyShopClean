using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class GetListOfProductViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ImageSrc { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int Count { get; set; }

    }
}
