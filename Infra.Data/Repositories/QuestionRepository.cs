using Domain.InterFaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Data.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {

        #region Injection
        private readonly AppWebContext _context;
        public QuestionRepository(AppWebContext context)
        {
            _context = context;
        }
        #endregion

        public async Task<Question> GetQuestionAsync(int id)
        {
            var question = await _context.Question.Include(p => p.User).Include(p=>p.Product)
                .Include(p => p.Replais).Where(p => p.Id == id).SingleOrDefaultAsync();

            return question;
        }

        public async Task<IEnumerable<Question>> GetQuestionsAsync()
        {
            var questions = await _context.Question.Include(p => p.User)
                .Include(p => p.Product).ThenInclude(p => p.ProductImages)
                .Include(p => p.Replais).Where(p=>p.ReplayOn==null).OrderByDescending(p=>p.Id).ToListAsync();

            return questions;
        }

        public async Task AddQuestionAsync(Question t)
        {
            await _context.Question.AddAsync(t);
        }


        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void DeleteQuestionsAsync(IEnumerable<Question> t)
        {
            _context.Question.RemoveRange(t);
        }
    }
}
