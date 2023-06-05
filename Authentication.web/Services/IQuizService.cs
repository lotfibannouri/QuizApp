using Authentication.web.Model;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace Authentication.web.Services
{
    public interface IQuizService
    {
        Task<Response> CreateQuiz(CreationQuizDTO quiz);
        Task<List<ListQuizDTO>> ListeQuiz();
        Task<Response> BindQuizToUser(QuizUserDTO QuizUser);
        Task<Response> BindQuizToQuestion(string IdQuiz, string IdQuestio);
        Task<Response> DeleteQuiz(string id);
        Task<ListQuizDTO> GetQuizById(string id);

    }
}
