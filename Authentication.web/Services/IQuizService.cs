using Authentication.web.Model;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace Authentication.web.Services
{
    public interface IQuizService
    {
        Task<Response> CreateQuiz(CreationQuizDTO quiz);
        Task<List<ListQuizDTO>> ListeQuiz();
        Task<Response> BindQuiz(QuizUserDTO QuizUser);
        Task<Response> DeleteQuiz(string id);

    }
}
