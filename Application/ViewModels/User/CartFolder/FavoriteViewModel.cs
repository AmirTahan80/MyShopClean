using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.User
{
    public class FavoriteViewModel
    {
        public int Id { get; set; }

        public IEnumerable<FavoriteDetailViewModel> FavoriteDetails { get; set; }
    }
}
