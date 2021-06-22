using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Factor
    {
        public int FactorId { get; set; }
        public int TrackingCode { get; set; }
        public bool IsRequestPay { get; set; }
        public DateTime? PayDate { get; set; }
        public int TotalPrice { get; set; }
        public string UserDetailId { get; set; }

        //Navigation
        public OrderStatus FactorStatus { get; set; }
        public ICollection<FactorDetail> FactorDetails { get; set; }
        public ApplicationUser UserDetailPaying { get; set; }

    }
}
