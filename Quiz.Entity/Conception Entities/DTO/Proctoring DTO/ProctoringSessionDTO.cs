using System;

namespace QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO
{
    /// <summary>Une ligne de la grille de consultation : un passage de test surveillé.</summary>
    public class ProctoringSessionDTO
    {
        public string SessionId { get; set; }
        public string QuizId { get; set; }
        public string QuizTitre { get; set; }
        public string UserId { get; set; }

        /// <summary>Résolu côté client via IAdministrationService : l'API des quiz ignore les utilisateurs.</summary>
        public string? UserName { get; set; }

        public DateTime StartedAt { get; set; }
        public int SnapshotCount { get; set; }
        public int IncidentCount { get; set; }
    }
}
