using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.Admin
{
    public class CommentsViewModel
    {
        public int CommentId { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است")]
        [Display(Name ="نظر")]
        public string CommentText { get; set; }
        [Required(ErrorMessage = "فیلد {0} اجباری است")]
        [Display(Name = "موضوع")]
        public string CommentTopic { get; set; }
        public DateTime InsertTime { get; set; }
        public bool IsShow { get; set; }
        public bool Suggest { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int ProductId { get; set; }
        public IEnumerable<string> Bads { get; set; }
        public IEnumerable<string> Goodness { get; set; }
    }
}
