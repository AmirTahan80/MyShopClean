using Application.ViewModels.Admin;
using System.Threading.Tasks;

namespace Application.InterFaces.Admin
{
    public interface ISiteSettingService
    {
        Task<SiteSettingViewModel> GetAsync();
        Task UpdateAsync(SiteSettingViewModel model);
    }
}
