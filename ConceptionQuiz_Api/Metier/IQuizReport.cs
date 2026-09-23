using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.ReportDTO;
using System.Globalization;

namespace ConceptionQuiz_Api.Metier
{
    public interface IQuizReport
    {
        Task<double> CalculateMultiChoiceScore(ScoreRequestDTO request);
        Task<byte[]> GenerateQuizReportAsync(QuizReportRequestDTO request);
        Task<QuizScoreSummaryDTO> GetQuizScoreSummaryAsync(QuizReportRequestDTO request);

    }
}
