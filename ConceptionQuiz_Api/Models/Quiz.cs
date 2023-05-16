namespace ConceptionQuiz_Api.Models
{
    public class Quiz
    {
        public string id { get; set; }
        public string titre { get; set; }
        public string quiz { get; set; }
        public int niv_deficulte { get; set; }
        public int duree_quiz { get; set; }
        public DateTime dateCreation { get; set; }
        public int nbr_questions { get; set; }


    }
}
