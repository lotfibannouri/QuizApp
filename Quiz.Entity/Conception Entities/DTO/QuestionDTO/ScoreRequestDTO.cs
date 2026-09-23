using System.Collections.Generic;

namespace QuizApp.Entities.Conception_Entities.DTO.QuestionDTO
{
    public class AnswerSubmissionDTO
    {
        public string Body { get; set; }
        public bool IsChecked { get; set; }
    }

    public class ScoreRequestDTO
    {
        public string QuestionId { get; set; }
        public List<AnswerSubmissionDTO>? Answers { get; set; }
        public string? SubmittedCode { get; set; }
        public string? SubmittedOutput { get; set; }
    }
}
