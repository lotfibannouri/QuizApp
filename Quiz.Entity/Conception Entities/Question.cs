using QuizApp.Entities.Base_Entities;
using QuizApp.Entities.Conception_Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities
{
    public class Question : BaseEntity
    {
        public string questionText { get; set; }
        public string? description { get; set; }
        public string type { get; set; }
        public int note { get; set; }
        public Guid? categorieId { get; set; }
        public Categorie? categorie { get; set; }
        public  ICollection<Quiz>? quiz { get; set; }
        public  ICollection<Reponse>? reponses { get; set; }
        
        public  ICollection<Proposition>? propositions { get; set; }
    }
}
