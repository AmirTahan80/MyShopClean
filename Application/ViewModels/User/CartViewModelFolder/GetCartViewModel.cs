using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Application.ViewModels.Admin;

namespace Application.ViewModels.User
{
    public class GetCartViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int CartId { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        [Required]
        public decimal TotlaPrice { get; set; }
        public ICollection<GetCartDetailViewModel> CartDetails{get; set;}
    }
}