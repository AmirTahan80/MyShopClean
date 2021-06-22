using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class HomePageViewModel
    {
        public ICollection<IndexViewModel> IndexImages { get; set; }
        public IEnumerable<GetProductForUserViewModel> Products { get; set; }
        public string AdminPhone { get; set; }
    }
}
