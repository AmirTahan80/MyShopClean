using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class GetProductCommentViewModel
    {
        #region Propeties
        [Required]
        public int CommentId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int? ReplyId { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="نام کاربری")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name ="متن پیام")]
        [MaxLength(1000,ErrorMessage ="بیشتر از 1000 کاراکتر است")]
        public string CommentText { get; set; }
        #endregion
        #region Navigation
        public ICollection<GetProductCommentViewModel> Replies { get; set; }
        #endregion
    }
}
