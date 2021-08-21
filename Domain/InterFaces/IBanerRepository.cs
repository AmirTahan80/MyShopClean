using Domain.Models.IndexFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.InterFaces
{
    public interface IBanerRepository:ISaveInterFaces
    {
        Task<IEnumerable<Baner>> GetBanersAsync();
        Task AddBanerAsync(Baner t);
        void DeleteBaner(Baner t);
    }
}
