using ConceptionQuiz_Api.Models;
using ConceptionQuiz_Api.Repository;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO;

namespace ConceptionQuiz_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProctoringController : ControllerBase
    {
        private readonly IProctoringRepository _proctoringRepository;

        public ProctoringController(IProctoringRepository proctoringRepository)
        {
            _proctoringRepository = proctoringRepository;
        }

        [HttpPost("SaveSnapshot")]
        public async Task<Response> SaveSnapshot([FromBody] ProctoringSnapshotDTO snapshot)
        {
            try
            {
                var result = await _proctoringRepository.SaveSnapshot(snapshot);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("LogIncident")]
        public async Task<Response> LogIncident([FromBody] ProctoringIncidentDTO incident)
        {
            try
            {
                var result = await _proctoringRepository.LogIncident(incident);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("ListSessions")]
        public async Task<List<ProctoringSessionDTO>> ListSessions()
        {
            try
            {
                var result = await _proctoringRepository.ListSessions();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("GetSessionEvents")]
        public async Task<List<ProctoringEventDTO>> GetSessionEvents(string sessionId)
        {
            try
            {
                var result = await _proctoringRepository.GetSessionEvents(sessionId);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Seule porte d'entrée vers les images : le dossier des captures n'est pas servi
        // en fichiers statiques, c'est donc ici qu'un contrôle d'accès pourra être branché.
        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string eventId)
        {
            try
            {
                var bytes = await _proctoringRepository.GetImage(eventId);
                if (bytes == null)
                    return NotFound("capture introuvable");

                return File(bytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("PurgeSession")]
        public async Task<Response> PurgeSession(string sessionId)
        {
            try
            {
                var result = await _proctoringRepository.PurgeSession(sessionId);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
