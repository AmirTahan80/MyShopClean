using Domain.InterFaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infra.Data.Repositories
{
    public class PayRepository : IPayRepository
    {
        #region Injections
        private readonly AppWebContext _context;
        public PayRepository(AppWebContext context)
        {
            _context = context;
        }

        #endregion
        public async Task<IEnumerable<RequestPay>> GetRequestPaiesAsync()
        {
            var requests = await _context.RequestPays.Include(p => p.ApplicationUser).ThenInclude(p=>p.UserDetail)
                .Include(p => p.Cart).ToListAsync();

            return requests;
        }

        public async Task AddRequestPay(RequestPay t)
        {
            await _context.RequestPays.AddAsync(t);
        }

        public async Task<IEnumerable<Factor>> GetFactors()
        {
            var factors = await _context.Factors.Include(p => p.FactorDetails).ToListAsync();

            return factors;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task AddFactor(Factor t)
        {
            await _context.Factors.AddAsync(t);
        }

        public async Task AddFactorDetails(IEnumerable<FactorDetail> t)
        {
            await _context.FactorDetails.AddRangeAsync(t);
        }

    }
}
