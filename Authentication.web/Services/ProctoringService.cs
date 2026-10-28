using Authentication.web.Model;
using QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO;
using System.Net.Http.Json;

namespace Authentication.web.Services
{
    public class ProctoringService : IProctoringService
    {
        private readonly HttpClient _httpClient;

        public ProctoringService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Response> SaveSnapshot(ProctoringSnapshotDTO snapshot)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Proctoring/SaveSnapshot", snapshot);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public async Task<Response> LogIncident(ProctoringIncidentDTO incident)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Proctoring/LogIncident", incident);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public async Task<List<ProctoringSessionDTO>> ListSessions()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Proctoring/ListSessions");
            List<ProctoringSessionDTO> sessions = await httpResponseMessage.Content.ReadFromJsonAsync<List<ProctoringSessionDTO>>();
            return sessions;
        }

        public async Task<List<ProctoringEventDTO>> GetSessionEvents(string sessionId)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Proctoring/GetSessionEvents?sessionId=" + sessionId);
            List<ProctoringEventDTO> events = await httpResponseMessage.Content.ReadFromJsonAsync<List<ProctoringEventDTO>>();
            return events;
        }

        public async Task<Response> PurgeSession(string sessionId)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("/api/Proctoring/PurgeSession?sessionId=" + sessionId, null);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public string GetImageUrl(string eventId)
            => new Uri(_httpClient.BaseAddress, "/api/Proctoring/GetImage?eventId=" + eventId).ToString();
    }
}
