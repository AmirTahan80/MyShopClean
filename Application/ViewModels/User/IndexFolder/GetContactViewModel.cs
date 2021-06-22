using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class GetContactViewModel
    {
        public string AdminEmail { get; set; }
        public string AdminPhoneNumber { get; set; }
    }
}
