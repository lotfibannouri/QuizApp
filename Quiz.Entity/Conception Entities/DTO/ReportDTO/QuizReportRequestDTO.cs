using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities.DTO.ReportDTO
{
    public class QuizReportRequestDTO
    {
        public string QuizId { get; set; }

        public List<ScoreRequestDTO> ListScoreRequest { get; set; }

        // Renseignés à la soumission d'un test (enregistrement du résultat) ; inutiles pour
        // le seul calcul du score ou la génération du rapport PDF.
        public string? UserId { get; set; }
        public string? SessionId { get; set; }
        public DateTime? StartedAt { get; set; }
        public bool IsAutoSubmitted { get; set; }
    }
}
