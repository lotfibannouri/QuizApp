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
    }
}
