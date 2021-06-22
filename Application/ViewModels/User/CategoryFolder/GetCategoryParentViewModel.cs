using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class GetCategoryParentViewModel
    {
        [Required]
        public int ParentId { get; set; }
        [Required]
        public string ParentName { get; set; }
    }
}