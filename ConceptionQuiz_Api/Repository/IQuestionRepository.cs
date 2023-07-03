using ConceptionQuiz_Api.Models;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace ConceptionQuiz_Api.Repository
{
    public interface IQuestionRepository
    {
        Task<Response> CreateQuestion(Question Question);
        Task<Response> DeleteQuestion(string id);
        Task<Response> UpdateQuestion(string id, Question Question);
        Task<List<Question>> ListQuestion();
        Task<Question> GetQuestionById(string id);
        Task<Question> GetQuestionByName(string name);
        Task<List<Question>> GetQuestionsByQuizId(string quizId);
      

    }
}
