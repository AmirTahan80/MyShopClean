using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        public int? ReplayId { get; set; }

        [Required]
        public string CommentTopic { get; set; }

        [Required]
        public string CommentText { get; set; }
        [Required]
        public DateTime CommentInsertTime { get; set; }

        public string ProductGoodNess { get; set; }
        public string ProductBads { get; set; }

        public bool IsShow { get; set; }

        public bool Suggest { get; set; }

        //Navigation
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [ForeignKey("ReplayId")]
        public Comment Parent { get; set; }
        public ICollection<Comment> Replies { get; set; }
    }
}
