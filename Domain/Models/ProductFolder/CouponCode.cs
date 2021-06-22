using System;

namespace Domain.Models
{
    public class CouponCode
    {
        public int CodeId { get; set; }
        public string CodeName { get; set; }
        public decimal CodePrice { get; set; }
        public DateTime IndertTimeCode { get; set; }
        public DateTime ExpireTimeCode { get; set; }
    }
}