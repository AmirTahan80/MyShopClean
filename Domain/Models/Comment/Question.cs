using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Topic { get; set; }
        [MaxLength(200)]
        [Required]
        public string QuestionText { get; set; }



        //Navigation
        public Product Product { get; set; }
        public ApplicationUser User { get; set; }
        public Question ReplayOn { get; set; }
        public ICollection<Question> Replais { get; set; }
    }
}
