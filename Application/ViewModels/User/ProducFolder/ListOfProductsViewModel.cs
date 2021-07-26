using System.Collections.Generic;

namespace Application.ViewModels.User
{
    public class ListOfProductsViewModel
    {
        public IEnumerable<GetListOfProductViewModel>  NewstProducts { get; set; }
        public IEnumerable<GetListOfProductViewModel>  CheapestProducts { get; set; }
        public IEnumerable<GetListOfProductViewModel>  MostSalerProducts { get; set; }
    }
}
