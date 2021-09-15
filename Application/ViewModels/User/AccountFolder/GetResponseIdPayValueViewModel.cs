using System;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.User
{
    public class GetResponseIdPayValueViewModel
    {
        [Required]
        public int status { get; set; }
        [Required]
        public int track_id { get; set; }
        [Required]
        public string id { get; set; }
        [Required]
        public string order_id { get; set; }
        [Required]
        public int amount { get; set; }
        [Required]
        public string card_no { get; set; }
        [Required]
        public string date { get; set; }
    }
}
