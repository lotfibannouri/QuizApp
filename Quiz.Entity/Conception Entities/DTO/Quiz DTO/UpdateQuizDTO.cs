using QuizApp.Entities.Base_DTO;

namespace QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO
{
    public class UpdateQuizDTO : BaseUpdateDTO
    {
        public string titre { get; set; }
        public string description { get; set; }
        public int niv_deficulte { get; set; }
        public int duree_quiz { get; set; }
        public int nbr_questions { get; set; }
    }
}
