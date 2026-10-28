namespace QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO
{
    /// <summary>Un quiz assigné à un utilisateur (ligne de QuizUser enrichie du quiz).</summary>
    public class QuizAssignmentDTO
    {
        public string QuizId { get; set; }
        public string QuizTitre { get; set; }
        public string UserId { get; set; }
        public int NbQuestionsPrevues { get; set; }
        public int NbQuestionsRattachees { get; set; }
    }
}
