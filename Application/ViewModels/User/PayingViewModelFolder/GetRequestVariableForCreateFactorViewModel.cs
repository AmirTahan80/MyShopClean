using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.Admin
{
    public class GetRequestVariableForCreateFactorViewModel
    {
        public int TrackingCode { get; set; }
        public bool IsRequestPay { get; set; }
        public DateTime? PayDate { get; set; }
        public int TotalPrice { get; set; }
        public string UserId { get; set; }
        public OrderStatus FactorStatus { get; set; }

        //Navigation Bar
        public ICollection<GetCartDetailForShowInFactorViewModel> CartDetails { get; set; }
        public GetUserDetailForShowInFactorViewModel UserPay { get; set; }

    }
}
