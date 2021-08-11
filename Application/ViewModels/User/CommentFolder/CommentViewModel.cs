using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class CommentViewModel
    {

        [Required(ErrorMessage = "عنوان نظر احباری است !!")]
        [Display(Name = "عنوان نظر")]
        public string Topic { get; set; }

        [Required(ErrorMessage ="متن نظر اجباری است !!")]
        [Display(Name ="نظر")]
        public string Text { get; set; }


        public int ProductId { get; set; }
        public int ReplayId { get; set; }

        public string ProductName { get; set; }
        public string ProductImage { get; set; }

        public bool CustomerSuggestToBuyThisProduct { get; set; }

        public List<string> Goodness { get; set; }
        public List<string> Bads { get; set; }
    }
}
