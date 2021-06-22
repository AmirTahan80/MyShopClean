using System;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class GetCategoryChildViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string CategoryName { get; set; }
    }
}
