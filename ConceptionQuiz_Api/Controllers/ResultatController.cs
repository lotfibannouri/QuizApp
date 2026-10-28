using ConceptionQuiz_Api.Metier;
using ConceptionQuiz_Api.Repository;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.ReportDTO;
using QuizApp.Entities.Conception_Entities.DTO.ResultatDTO;

namespace ConceptionQuiz_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultatController : ControllerBase
    {
        private readonly IResultatRepository _resultatRepository;
        private readonly IQuizReport _quizReport;

        public ResultatController(IResultatRepository resultatRepository, IQuizReport quizReport)
        {
            _resultatRepository = resultatRepository;
            _quizReport = quizReport;
        }

        /// <summary>
        /// Soumission d'un test : le score est calculé ici, à partir des réponses, puis
        /// enregistré avec son détail par question. Une même session n'est enregistrée
        /// qu'une fois : un second envoi renvoie le résultat déjà calculé.
        /// </summary>
        [HttpPost("Submit")]
        public async Task<ActionResult<QuizScoreSummaryDTO>> Submit([FromBody] QuizReportRequestDTO request)
        {
            if (!Guid.TryParse(request.SessionId, out var sessionId) || !Guid.TryParse(request.UserId, out _))
                return BadRequest("SessionId et UserId sont obligatoires.");

            try
            {
                var resultat = await _resultatRepository.GetBySession(sessionId);
                if (resultat == null)
                {
                    var data = await _quizReport.BuildReportDataAsync(request);
                    resultat = await _resultatRepository.SaveResultat(request, data);
                }

                return ToSummary(resultat);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("ListByUser")]
        public async Task<List<ResultatDTO>> ListByUser(string userId)
        {
            try
            {
                return await _resultatRepository.ListByUser(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("ListByQuiz")]
        public async Task<List<ResultatDTO>> ListByQuiz(string quizId)
        {
            try
            {
                return await _resultatRepository.ListByQuiz(quizId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("ListAll")]
        public async Task<List<ResultatDTO>> ListAll()
        {
            try
            {
                return await _resultatRepository.ListAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("GetDetail")]
        public async Task<ActionResult<ResultatDetailDTO>> GetDetail(string resultatId)
        {
            if (!Guid.TryParse(resultatId, out _))
                return BadRequest("identifiant de résultat invalide");

            try
            {
                var detail = await _resultatRepository.GetDetail(resultatId);
                if (detail == null)
                    return NotFound("résultat introuvable");

                return detail;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        private static QuizScoreSummaryDTO ToSummary(Resultat resultat) => new QuizScoreSummaryDTO
        {
            Score = resultat.score,
            MaxScore = resultat.scoreMax,
            Percentage = resultat.pourcentage,
            Mention = resultat.mention
        };
    }
}
