using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Comment
    {
        #region Propeties
        public long CommentId { get; set; }
        public long ProductId { get; set; }
        public long? ReplyId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentInsertTime { get; set; }
        public bool IsShow { get; set; }
        public bool IsEdited { get; set; }
        public string ProductName { get; set; }
        public string ProductFirstImage { get; set; }
        #endregion
        #region NaviGation
        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
        public Comment ProductComment { get; set; }
        public ICollection<Comment> Replies { get; set; }
        #endregion
    }
}
