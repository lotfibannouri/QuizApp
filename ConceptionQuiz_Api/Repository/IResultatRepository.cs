using ConceptionQuiz_Api.Metier;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.ReportDTO;
using QuizApp.Entities.Conception_Entities.DTO.ResultatDTO;

namespace ConceptionQuiz_Api.Repository
{
    public interface IResultatRepository
    {
        Task<Resultat?> GetBySession(Guid sessionId);
        Task<Resultat> SaveResultat(QuizReportRequestDTO request, QuizReport.QuizReportData data);
        Task<List<ResultatDTO>> ListByUser(string userId);
        Task<List<ResultatDTO>> ListByQuiz(string quizId);
        Task<List<ResultatDTO>> ListAll();
        Task<ResultatDetailDTO?> GetDetail(string resultatId);
    }
}
