using QuizApp.Entities.Base_DTO;

namespace QuizApp.Entities.Conception_Entities.DTO.QuestionsQuizDTO
{
    public class QuestionsQuizDeleteDTO: BaseDeleteDTO
    {
        public virtual ICollection<Question> questions { get; set; }
    }
}
