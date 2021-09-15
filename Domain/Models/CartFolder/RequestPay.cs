using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class RequestPay
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime? DatePay { get; set; }
        [Required]
        public int Amount { get; set; }
        public bool IsPay { get; set; }
        public int RefId { get; set; }

        [Required]
        public string IdReturnIdPay { get; set; }
        [Required]
        public string ReturnLinkIdPay { get; set; }


        //Navigation Bar
        public ApplicationUser ApplicationUser { get; set; }
        public Cart Cart { get; set; }
    }
}
