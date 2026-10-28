using System;

namespace QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO
{
    public class ProctoringSnapshotDTO
    {
        public string QuizId { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public DateTime CapturedAt { get; set; }

        /// <summary>Image JPEG encodée en base64 (sans le préfixe data:image/jpeg;base64,).</summary>
        public string ImageBase64 { get; set; }
    }
}
