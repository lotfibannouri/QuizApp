using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using System.Threading.Tasks;

namespace Authentication.web.utility
{
    public interface IQuestionPersist
    {
        public Task<double> save();
    }
}
