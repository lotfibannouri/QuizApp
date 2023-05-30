using Authentication.web.Model;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace Authentication.web.Services
{
    public interface IQuestionService
    {
        Task<Response> CreateQuestion(CreationQ_PropDTO question);
    }
}
