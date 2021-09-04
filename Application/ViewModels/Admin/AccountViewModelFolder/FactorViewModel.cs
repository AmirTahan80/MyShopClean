using System.Collections.Generic;

namespace Application.ViewModels.Admin
{
    public class FactorViewModel
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string PhoneNumber { get; set; }
        public IList<string> DisCountNames { get; set; }
        public IList<int> DisCountPrices { get; set; }
        public bool UseDisCount { get; set; }
        public string FactorStatus { get; set; }
        public int RefId { get; set; }
        public int TotalPrice { get; set; }

        //Navigation
        public IEnumerable<FactorDetailViewModel> FactorDetails { get; set; }
    }
}
