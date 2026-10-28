using System;

namespace QuizApp.Entities.Conception_Entities.DTO.ResultatDTO
{
    /// <summary>Ligne de liste d'un résultat (historique d'un étudiant ou d'un quiz).</summary>
    public class ResultatDTO
    {
        public string Id { get; set; }
        public string? QuizId { get; set; }
        public string QuizTitre { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public int Tentative { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int DureeSecondes { get; set; }
        public bool IsAutoSubmitted { get; set; }
        public double Score { get; set; }
        public double ScoreMax { get; set; }
        public double Pourcentage { get; set; }
        public string Mention { get; set; }
        public bool IsReussi { get; set; }
        public int NbQuestions { get; set; }
        public int NbCorrectes { get; set; }
    }
}
