using Authentication.web.utility;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.ReportDTO;

namespace Authentication.web.Services
{
    public interface IReportService
    {
        Task<byte[]> GetReport(QuizReportRequestDTO RequestRequest);
        Task<QuizScoreSummaryDTO> GetScoreSummary(QuizReportRequestDTO RequestRequest);
    }
}
 