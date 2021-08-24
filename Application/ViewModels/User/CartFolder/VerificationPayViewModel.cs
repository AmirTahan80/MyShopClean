using System.Collections.Generic;

namespace Application.ViewModels.User
{
    public class VerificationPayViewModel
    {
        public string Id { get; set; }
        public int RefId { get; set; }
        public string UserEmail { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserAddress { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserPostCode { get; set; }
        public int TotalPrice { get; set; }


        //Navigation
        public ResultDto RetrunResult { get; set; }
        public IEnumerable<DisCountViewModel> DisCounts { get; set; }
        public IEnumerable<VerficationProductsViewModel> Products { get; set; }
    }
}
