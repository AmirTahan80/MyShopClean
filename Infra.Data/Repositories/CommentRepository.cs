using Domain.InterFaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        #region Injections
        private readonly AppWebContext _context;
        public CommentRepository(AppWebContext context)
        {
            _context = context;
        }
        #endregion
        public async Task<Comment> GetCommentAsync(int id)
        {
            var comment = await _context.Comments
                .Include(p=>p.Product).ThenInclude(p=>p.ProductImages)
                .Include(p=>p.User)
                .SingleOrDefaultAsync(p => p.CommentId == id);

            return comment;
        }
        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            var comments = await _context.Comments.Include(p=>p.Product).ThenInclude(p=>p.ProductImages)
                .Include(p=>p.User).ToListAsync();

            return comments;
        }

        public async Task AddCommentAsync(Comment t)
        {
            await _context.Comments.AddAsync(t);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
