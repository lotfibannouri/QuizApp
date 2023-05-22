using QuizApp.Entities.Base_Entities;
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
        public string type { get; set; }
        public string body { get; set; }
        public virtual ICollection<Quiz>? quiz { get; set; }
        public virtual ICollection<Reponse>? reponses { get; set; }
    }
}
