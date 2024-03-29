﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class ProfileViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="فیلد {0} احباری است ")]
        [Display(Name ="نام کاربری")]
        public string Name { get; set; }
        [Required(ErrorMessage ="فیلد {0} احباری است ")]
        [Display(Name = "پست الکترونیک")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string RoleName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public bool IsEmailConfirm { get; set; }

        public bool IsInNews { get; set; } = false;

        public IEnumerable<UserCommentsViewModel> Comments { get; set; }
        public IEnumerable<QuestionViewModel> Questions { get; set; }
        public IEnumerable<FactorViewModel> Factors { get; set; }
        public FactorViewModel Factor { get; set; }
    }
}
