using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class GetProductDescriptionViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool IsProductHaveAttributes { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public int CategoryId { get; set; }

        public int TemplateId { get; set; }

        public bool IsInUserFavorite { get; set; } = false;
        public bool InstagramPost { get; set; }

        //Navigation
        public IEnumerable<ProductAttributeViewModel> Attributes { get; set; }
        public IEnumerable<ProductImageViewModel> Images { get; set; }
        public IEnumerable<ProductAttributesTemplate> Templates { get; set; }
        public IEnumerable<ProductCommentViewModel> Comments { get; set; }
        public IEnumerable<QuestionViewModel> Questions { get; set; }
        public IEnumerable<PropertiesViewModel> Properties { get; set; }

        public CommentViewModel Comment { get; set; }
        public QuestionViewModel Question { get; set; }

    }
}
