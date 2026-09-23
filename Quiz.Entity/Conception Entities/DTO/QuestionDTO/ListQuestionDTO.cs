using QuizApp.Entities.Base_DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities.DTO.QuestionDTO
{
    public class ListQuestionDTO : BaseListDTO
    {
        public string questionText { get; set; }
        public string? description { get; set; }
        public string type { get; set; }
        public int note { get; set; }
        public string categorieId { get; set; }
        public string categorieTitre { get; set; }
        public ICollection<Proposition>? propositions { get; set; }
        public ICollection<Reponse>? reponses { get; set; }
    }
}
