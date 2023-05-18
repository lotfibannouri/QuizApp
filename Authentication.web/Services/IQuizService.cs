using Authentication.web.Model;

namespace Authentication.web.Services
{
    public interface IQuizService
    {
        Task<Response> CreateQuiz(Quiz quiz);

    }
}
