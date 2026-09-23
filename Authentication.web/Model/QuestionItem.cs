using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;

namespace Authentication.web.Model
{
    public class QuestionItem
    {
        public QuestionItem(ListQuestionDTO question, bool isBound)
        {
            Question = question;
            _Identifier = isBound ? "Assigned" : "Available";
        }

        public ListQuestionDTO Question { get; set; }
        public string _Identifier { get; set; }
    }
}
