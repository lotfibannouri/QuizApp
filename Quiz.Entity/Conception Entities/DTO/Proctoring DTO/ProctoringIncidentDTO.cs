using System;

namespace QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO
{
    public class ProctoringIncidentDTO
    {
        public string QuizId { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public DateTime CapturedAt { get; set; }

        /// <summary>TabBlur ou CameraDenied.</summary>
        public string Type { get; set; }

        public string? Details { get; set; }
    }
}
