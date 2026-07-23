using Domain.Models;
using System.Threading.Tasks;

namespace Domain.InterFaces
{
    public interface ISiteSettingRepository
    {
        Task<SiteSetting> GetAsync();
        Task SaveAsync(SiteSetting setting);
    }
}
