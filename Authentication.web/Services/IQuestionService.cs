using Authentication.web.Model;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace Authentication.web.Services
{
    public interface IQuestionService
    {
        Task<Response> CreateQuestion(CreationQuestionDTO question);

        Task<Response> UpdateQuestion(string id, CreationQuestionDTO question);

        Task<Response> DeleteQuestion(string id);

        Task<List<ListQuestionDTO>> GetQuestions();

        Task<List<ListQuestionDTO>> GetQuestionsByQuizId(string QuizId);

        Task<ListQuestionDTO> GetQuestionsById(string questionId);

        Task<double> CalculateMultiChoiceScore(ScoreRequestDTO request);

    }
}
