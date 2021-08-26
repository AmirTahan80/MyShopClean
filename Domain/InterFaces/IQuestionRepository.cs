using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.InterFaces
{
    public interface IQuestionRepository : ISaveInterFaces
    {
        Task<Question> GetQuestionAsync(int id);
        Task<IEnumerable<Question>> GetQuestionsAsync();
        Task AddQuestionAsync(Question t);
        void DeleteQuestionsAsync(IEnumerable<Question> t);
    }
}
