using ConceptionQuiz_Api.Models;
using Microsoft.EntityFrameworkCore;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO;

namespace ConceptionQuiz_Api.Repository
{
    public class ProctoringRepository : IProctoringRepository
    {
        private const string SnapshotType = "Snapshot";

        private readonly ConceptionQuizDbContext _dbContext;
        private readonly string _capturesRoot;

        public ProctoringRepository(ConceptionQuizDbContext dbContext, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;

            // Les captures vivent sous le ContentRoot (dossier de l'application) et jamais sous
            // un wwwroot : rien n'y est exposé en HTTP, les images ne sortent que par le contrôleur.
            var configured = configuration["Proctoring:CapturesRoot"] ?? "ProctoringCaptures";
            _capturesRoot = Path.IsPathRooted(configured)
                ? configured
                : Path.Combine(environment.ContentRootPath, configured);
        }

        public async Task<Response> SaveSnapshot(ProctoringSnapshotDTO snapshot)
        {
            if (string.IsNullOrWhiteSpace(snapshot.ImageBase64))
                return new Response(false, "image de surveillance vide...");

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(StripDataUrlPrefix(snapshot.ImageBase64));
            }
            catch (FormatException)
            {
                return new Response(false, "image de surveillance illisible...");
            }

            var sessionDirectory = Path.Combine(_capturesRoot, snapshot.QuizId, snapshot.SessionId);
            Directory.CreateDirectory(sessionDirectory);

            var fileName = $"{snapshot.CapturedAt:yyyyMMdd_HHmmss_fff}.jpg";
            await File.WriteAllBytesAsync(Path.Combine(sessionDirectory, fileName), bytes);

            // Seul le chemin relatif est stocké : la racine peut changer d'une machine à l'autre.
            var relativePath = Path.Combine(snapshot.QuizId, snapshot.SessionId, fileName);

            await _dbContext.proctoringEvents.AddAsync(new ProctoringEvent
            {
                quizid = new Guid(snapshot.QuizId),
                userid = new Guid(snapshot.UserId),
                sessionId = new Guid(snapshot.SessionId),
                capturedAt = snapshot.CapturedAt,
                type = SnapshotType,
                filePath = relativePath,
                Created = DateTime.Now,
                Modified = DateTime.Now
            });

            int rowsAffected = await _dbContext.SaveChangesAsync();
            if (rowsAffected > 0)
                return new Response(true, "capture enregistrée");
            else
                return new Response(false, "enregistrement de la capture a été échoué...");
        }

        public async Task<Response> LogIncident(ProctoringIncidentDTO incident)
        {
            await _dbContext.proctoringEvents.AddAsync(new ProctoringEvent
            {
                quizid = new Guid(incident.QuizId),
                userid = new Guid(incident.UserId),
                sessionId = new Guid(incident.SessionId),
                capturedAt = incident.CapturedAt,
                type = incident.Type,
                filePath = null,
                details = incident.Details,
                Created = DateTime.Now,
                Modified = DateTime.Now
            });

            int rowsAffected = await _dbContext.SaveChangesAsync();
            if (rowsAffected > 0)
                return new Response(true, "incident enregistré");
            else
                return new Response(false, "enregistrement de l'incident a été échoué...");
        }

        public async Task<List<ProctoringSessionDTO>> ListSessions()
        {
            // Agrégé côté SQL Server : une ligne par session, quel que soit le nombre de captures.
            // Une session appartient toujours à un seul quiz et un seul étudiant, les ajouter à la
            // clé de regroupement ne change donc rien au résultat.
            var sessions = await _dbContext.proctoringEvents
                .GroupBy(e => new { e.sessionId, e.quizid, e.userid })
                .Select(g => new
                {
                    g.Key.sessionId,
                    g.Key.quizid,
                    g.Key.userid,
                    StartedAt = g.Min(e => e.capturedAt),
                    SnapshotCount = g.Count(e => e.type == SnapshotType),
                    IncidentCount = g.Count(e => e.type != SnapshotType)
                })
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();

            var quizTitres = await _dbContext.quiz.ToDictionaryAsync(q => q.Id, q => q.titre);

            return sessions
                .Select(s => new ProctoringSessionDTO
                {
                    SessionId = s.sessionId.ToString(),
                    QuizId = s.quizid.ToString(),
                    QuizTitre = quizTitres.TryGetValue(s.quizid, out var titre) ? titre : "",
                    UserId = s.userid.ToString(),
                    StartedAt = s.StartedAt,
                    SnapshotCount = s.SnapshotCount,
                    IncidentCount = s.IncidentCount
                })
                .ToList();
        }

        public async Task<List<ProctoringEventDTO>> GetSessionEvents(string sessionId)
        {
            var sessionGuid = new Guid(sessionId);

            return await _dbContext.proctoringEvents
                .Where(e => e.sessionId == sessionGuid)
                .OrderBy(e => e.capturedAt)
                .Select(e => new ProctoringEventDTO
                {
                    Id = e.Id.ToString(),
                    CapturedAt = e.capturedAt,
                    Type = e.type,
                    HasImage = e.filePath != null,
                    Details = e.details
                })
                .ToListAsync();
        }

        public async Task<byte[]?> GetImage(string eventId)
        {
            var proctoringEvent = await _dbContext.proctoringEvents
                .SingleOrDefaultAsync(e => e.Id == new Guid(eventId));

            if (proctoringEvent?.filePath == null)
                return null;

            var fullPath = Path.Combine(_capturesRoot, proctoringEvent.filePath);
            if (!File.Exists(fullPath))
                return null;

            return await File.ReadAllBytesAsync(fullPath);
        }

        public async Task<Response> PurgeSession(string sessionId)
        {
            var sessionGuid = new Guid(sessionId);
            var events = await _dbContext.proctoringEvents
                .Where(e => e.sessionId == sessionGuid)
                .ToListAsync();

            if (!events.Any())
                return new Response(false, "session de surveillance introuvable...");

            int rowsAffected = await DeleteEvents(events);
            if (rowsAffected > 0)
                return new Response(true, "suppression des captures réussie");
            else
                return new Response(false, "suppression des captures a été échouée");
        }

        public async Task<int> PurgeSessionsOlderThan(DateTime limit)
        {
            // Une session n'est purgée que si sa dernière activité est antérieure à la limite :
            // un passage à cheval sur la limite garde toutes ses captures.
            var expiredSessionIds = await _dbContext.proctoringEvents
                .GroupBy(e => e.sessionId)
                .Where(g => g.Max(e => e.capturedAt) < limit)
                .Select(g => g.Key)
                .ToListAsync();

            if (!expiredSessionIds.Any())
                return 0;

            var events = await _dbContext.proctoringEvents
                .Where(e => expiredSessionIds.Contains(e.sessionId))
                .ToListAsync();

            await DeleteEvents(events);
            return expiredSessionIds.Count;
        }

        private async Task<int> DeleteEvents(List<ProctoringEvent> events)
        {
            // Le dossier d'une session est supprimé en entier : il n'est partagé avec aucune autre.
            foreach (var session in events.GroupBy(e => new { e.quizid, e.sessionId }))
            {
                var sessionDirectory = Path.Combine(_capturesRoot, session.Key.quizid.ToString(), session.Key.sessionId.ToString());
                if (Directory.Exists(sessionDirectory))
                    Directory.Delete(sessionDirectory, true);
            }

            _dbContext.proctoringEvents.RemoveRange(events);
            return await _dbContext.SaveChangesAsync();
        }

        private static string StripDataUrlPrefix(string imageBase64)
        {
            var separatorIndex = imageBase64.IndexOf("base64,", StringComparison.OrdinalIgnoreCase);
            return separatorIndex >= 0
                ? imageBase64.Substring(separatorIndex + "base64,".Length)
                : imageBase64;
        }
    }
}
