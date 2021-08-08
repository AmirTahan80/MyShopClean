using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class ResultDto
    {
        public bool Status { get; set; }
        public bool ShowNotFound { get; set; } = false;
        public string ErrorMessage { get; set; }
        public string SuccesMessage { get; set; }
    }
}
