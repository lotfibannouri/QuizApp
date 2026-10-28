using System.Collections.Generic;

namespace QuizApp.Entities.Conception_Entities.DTO.ResultatDTO
{
    /// <summary>Un passage de quiz avec le détail de ses questions.</summary>
    public class ResultatDetailDTO
    {
        public ResultatDTO Resultat { get; set; }
        public List<ResultatQuestionDTO> Questions { get; set; } = new();
    }

    public class ResultatQuestionDTO
    {
        public int Ordre { get; set; }
        public string QuestionTexte { get; set; }
        public string QuestionType { get; set; }
        public double Score { get; set; }
        public double ScoreMax { get; set; }
        public bool IsCorrecte { get; set; }
        public bool IsRepondue { get; set; }

        /// <summary>JSON des réponses (QCM, Vrai/Faux) ou code soumis (Codage).</summary>
        public string? ReponseDonnee { get; set; }

        public string? SortieObtenue { get; set; }
    }
}
