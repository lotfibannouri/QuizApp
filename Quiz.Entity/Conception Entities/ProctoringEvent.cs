using QuizApp.Entities.Base_Entities;
using System;

namespace QuizApp.Entities.Conception_Entities
{
    public class ProctoringEvent : BaseEntity
    {
        public Guid quizid { get; set; }

        public Guid userid { get; set; }

        /// <summary>
        /// Identifie un passage de test précis : l'application ne persiste aucune notion
        /// de tentative, donc sans lui les captures de deux passages du même quiz par le
        /// même étudiant seraient indiscernables.
        /// </summary>
        public Guid sessionId { get; set; }

        public DateTime capturedAt { get; set; }

        /// <summary>Snapshot, TabBlur ou CameraDenied.</summary>
        public string type { get; set; }

        /// <summary>Chemin relatif du JPEG ; nul pour un incident sans image.</summary>
        public string? filePath { get; set; }

        public string? details { get; set; }
    }
}
