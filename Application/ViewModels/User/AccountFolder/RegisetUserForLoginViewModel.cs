using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class RegisetUserForLoginViewModel
    {
        [Required(ErrorMessage ="فیلد {0} اجباری است !")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است !")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است !")]
        [DataType(DataType.Password)]
        public string PassWord { get; set; }
        [Required(ErrorMessage ="فیلد {0} اجباری است !")]
        [DataType(DataType.Password)]
        [Compare("PassWord")]
        public string RePassWord { get; set; }
    }
}
