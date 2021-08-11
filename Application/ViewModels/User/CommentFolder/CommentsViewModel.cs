using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class CommentsViewModel
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
    }
}
