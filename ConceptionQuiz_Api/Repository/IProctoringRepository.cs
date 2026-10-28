using ConceptionQuiz_Api.Models;
using QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO;

namespace ConceptionQuiz_Api.Repository
{
    public interface IProctoringRepository
    {
        Task<Response> SaveSnapshot(ProctoringSnapshotDTO snapshot);
        Task<Response> LogIncident(ProctoringIncidentDTO incident);
        Task<List<ProctoringSessionDTO>> ListSessions();
        Task<List<ProctoringEventDTO>> GetSessionEvents(string sessionId);
        Task<byte[]?> GetImage(string eventId);
        Task<Response> PurgeSession(string sessionId);
        Task<int> PurgeSessionsOlderThan(DateTime limit);
    }
}
