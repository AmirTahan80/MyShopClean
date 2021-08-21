using Application.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterFaces.User
{
    public interface IHomePageServices
    {
        Task<HomePageViewModel> GetBanersAndProductsAsync();
    }
}
