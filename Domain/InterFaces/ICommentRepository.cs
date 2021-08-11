using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.InterFaces
{
    public interface ICommentRepository : ISaveInterFaces
    {
        Task<Comment> GetCommentAsync(int id);

        Task<IEnumerable<Comment>> GetCommentsAsync();

        Task AddCommentAsync(Comment t);
    }
}
