using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.InterFaces
{
    public interface IContactUsRepository : ISaveInterFaces
    {
        Task<IEnumerable<ContactUs>> GetContactsUsAsync();
        Task AddContactUsAsync(ContactUs t);
        void RemoveContactUs(IEnumerable<ContactUs> t);
    }
}
