using System;

namespace QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO
{
    /// <summary>Un événement d'une session : photo ou incident.</summary>
    public class ProctoringEventDTO
    {
        public string Id { get; set; }
        public DateTime CapturedAt { get; set; }
        public string Type { get; set; }

        /// <summary>Vrai si l'événement porte une image consultable.</summary>
        public bool HasImage { get; set; }

        public string? Details { get; set; }
    }
}
