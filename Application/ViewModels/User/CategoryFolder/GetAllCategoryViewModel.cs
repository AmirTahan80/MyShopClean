using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class GetAllCategoryViewModel
    {
        #region Properties
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public bool HasChild { get; set; }
        #endregion
        #region NavigationBar
        public GetCategoryParentViewModel Parent { get; set; }
        #endregion
    }
}