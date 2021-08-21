using Domain.InterFaces;
using Domain.Models.IndexFolder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Data.Repositories
{
    public class BanerRepository : IBanerRepository
    {
        #region Injections
        private readonly AppWebContext _context;
        public BanerRepository(AppWebContext context)
        {
            _context = context;
        }
        #endregion



        public async Task<IEnumerable<Baner>> GetBanersAsync()
        {
            var baners = await _context.Baners.ToListAsync();

            return baners;
        }

        public async Task AddBanerAsync(Baner t)
        {
            await _context.Baners.AddAsync(t);
        }

        public void DeleteBaner(Baner t)
        {
            _context.Remove(t);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
