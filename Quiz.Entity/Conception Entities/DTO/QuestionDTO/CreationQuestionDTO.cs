using QuizApp.Entities.Base_DTO;
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
    }
}
