using ConceptionQuiz_Api.Models;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace ConceptionQuiz_Api.Repository
{
    public interface IQuizRepository
    {
        Task<Response> CreateQuiz(CreationQuizDTO quiz);
        Task<bool> DeleteQuiz(string id);
        Task<bool> UpdateQuiz(string id, Quiz quiz);
        Task<List<ListQuizDTO>> ListQuiz();
        Task<Quiz> GetQuizById(string id);
        Task<Quiz> GetQuizByName(string name);

        Task<bool> CreateQuestion(Question question);
        Task<List<Question>> ListQuestions();
        Task<Question> GetQuestionById(string id);
        Task<bool> DeleteQuestion(string id);
        Task<bool> AddToQuiz(string Idquestion, string Idquiz);
    }
}
