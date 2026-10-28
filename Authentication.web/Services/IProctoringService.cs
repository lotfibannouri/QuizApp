using Authentication.web.Model;
using QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO;

namespace Authentication.web.Services
{
    public interface IProctoringService
    {
        Task<Response> SaveSnapshot(ProctoringSnapshotDTO snapshot);

        Task<Response> LogIncident(ProctoringIncidentDTO incident);

        Task<List<ProctoringSessionDTO>> ListSessions();

        Task<List<ProctoringEventDTO>> GetSessionEvents(string sessionId);

        Task<Response> PurgeSession(string sessionId);

        /// <summary>URL de l'image d'un événement, à poser dans un attribut src.</summary>
        string GetImageUrl(string eventId);
    }
}
