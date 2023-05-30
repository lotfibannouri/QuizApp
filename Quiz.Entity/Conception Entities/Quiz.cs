using QuizApp.Entities.Base_Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp.Entities.Conception_Entities
{
    public class Quiz : BaseEntity
    {
        public string titre { get; set; }
        public string description { get; set; }
        public int niv_deficulte { get; set; }
        public int duree_quiz { get; set; }
        public int nbr_questions { get; set; }
        public ICollection<Question>? questions { get; set; }
    }
}
