using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.Admin
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Topic { get; set; }
        public string Text { get; set; }
        public bool IsSelected { get; set; }
        public string Awnser { get; set; }

        public ICollection<QuestionViewModel> Replaise { get; set; }
    }
}
