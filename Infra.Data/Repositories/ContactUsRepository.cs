using Domain.InterFaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Data.Repositories
{
    public class ContactUsRepository : IContactUsRepository
    {
        #region Injections
        private readonly AppWebContext _context;
        public ContactUsRepository(AppWebContext context)
        {
            _context = context;
        }
        #endregion


        public async Task<IEnumerable<ContactUs>> GetContactsUsAsync()
        {
            var contacts = await _context.ContactUs.OrderByDescending(p=>p.Id).Include(p=>p.User).ToListAsync();

            return contacts;
        }

        public async Task AddContactUsAsync(ContactUs t)
        {
            await _context.ContactUs.AddAsync(t);
        }

        public void RemoveContactUs(IEnumerable<ContactUs> t)
        {
            _context.ContactUs.RemoveRange(t);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<News>> GetAllEmailInNewsAsync()
        {
            var news = await _context.News.ToListAsync();
            return news;
        }
        public async Task JoinToNewsAsync(News t)
        {
            await _context.News.AddAsync(t);
        }

    }
}
