using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RequestForPay
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public long FactorId { get; set; }
        public DateTime? DatePay { get; set; }
        public int Amount { get; set; }
        public bool IsPay { get; set; }
        public int RefId { get; set; }

        //Navigation Bar
        public ApplicationUser ApplicationUser { get; set; }
    }
}
