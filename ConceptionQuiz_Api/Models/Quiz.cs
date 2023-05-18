using System.ComponentModel.DataAnnotations.Schema;

namespace ConceptionQuiz_Api.Models
{
    public class Quiz
    {
        public Quiz()
        {
            id = Guid.NewGuid().ToString();
            this.questions = new HashSet<Question>();
        }

        public  string id { get; set; }
        public string titre { get; set; }
        public string description { get; set; }
        public int niv_deficulte { get; set; }
        public int duree_quiz { get; set; }
        public DateTime dateCreation { get; set; }
        public int nbr_questions { get; set; }
        public virtual ICollection<Question>? questions { get; set; }

    }
}
