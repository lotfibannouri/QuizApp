using QuizApp.Entities.Base_DTO;

namespace QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO
{
    public class ListQuizDTO :BaseListDTO
    {
        public string titre { get; set; }
        public int niv_deficulte { get; set; }
        public int duree_quiz { get; set; }
    }
}
