using System.Collections.Generic;

namespace Application.ViewModels.User
{
    public class HomePageViewModel
    {
        public IEnumerable<GetListOfProductViewModel>  NewtProducts { get; set; }
        public IEnumerable<GetListOfProductViewModel>  CheapestProducts { get; set; }
        public IEnumerable<GetListOfProductViewModel>  MostSalerProducts { get; set; }
        public IEnumerable<BanerViewModel>  Baners { get; set; }
    }
}
