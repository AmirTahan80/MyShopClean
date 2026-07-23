using Domain.InterFaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infra.Data.Repositories
{
    public class SiteSettingRepository : ISiteSettingRepository
    {
        private readonly AppWebContext _context;

        public SiteSettingRepository(AppWebContext context)
        {
            _context = context;
        }

        public Task<SiteSetting> GetAsync() => _context.SiteSettings.SingleOrDefaultAsync();

        public async Task SaveAsync(SiteSetting setting)
        {
            if (setting.Id == 0)
            {
                await _context.SiteSettings.AddAsync(setting);
            }
            else
            {
                _context.SiteSettings.Update(setting);
            }

            await _context.SaveChangesAsync();
        }
    }
}
