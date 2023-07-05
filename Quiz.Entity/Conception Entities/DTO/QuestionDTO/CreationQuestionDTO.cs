using QuizApp.Entities.Base_DTO;
using QuizApp.Entities.Conception_Entities.DTO.Proposition_DTO;
using QuizApp.Entities.Conception_Entities.DTO.Reponse_DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities.DTO.QuestionDTO
{
    public class CreationQuestionDTO : BaseCreationDTO
    {
        public string questionText { get; set; }
        public string type { get; set; }
        public List<PropositionDTO>? propositions { get; set; }
        public List<CreationReponseDTO> reponses { get; set; }

    }
}
