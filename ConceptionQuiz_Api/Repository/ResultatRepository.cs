using ConceptionQuiz_Api.Metier;
using ConceptionQuiz_Api.Models;
using Microsoft.EntityFrameworkCore;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.ReportDTO;
using QuizApp.Entities.Conception_Entities.DTO.ResultatDTO;

namespace ConceptionQuiz_Api.Repository
{
    public class ResultatRepository : IResultatRepository
    {
        private const double SeuilReussite = 50;

        private readonly ConceptionQuizDbContext _dbContext;

        public ResultatRepository(ConceptionQuizDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Resultat?> GetBySession(Guid sessionId)
            => await _dbContext.resultats.SingleOrDefaultAsync(r => r.sessionId == sessionId);

        public async Task<Resultat> SaveResultat(QuizReportRequestDTO request, QuizReport.QuizReportData data)
        {
            var quiz = data.Quiz;
            var userId = new Guid(request.UserId);
            var submittedAt = DateTime.Now;
            // Sans heure de début (ancien client), la durée est inconnue : on la laisse à 0.
            var startedAt = request.StartedAt ?? submittedAt;

            int previousAttempts = await _dbContext.resultats
                .CountAsync(r => r.quizid == quiz.Id && r.userid == userId);

            var resultat = new Resultat
            {
                quizid = quiz.Id,
                quizTitre = quiz.titre ?? "",
                categorieId = quiz.categorieId,
                userid = userId,
                sessionId = new Guid(request.SessionId),
                tentative = previousAttempts + 1,
                startedAt = startedAt,
                submittedAt = submittedAt,
                dureeSecondes = (int)Math.Max(0, (submittedAt - startedAt).TotalSeconds),
                dureeAutoriseeMinutes = quiz.duree_quiz,
                isAutoSubmitted = request.IsAutoSubmitted,
                score = data.Total,
                scoreMax = data.MaxTotal,
                pourcentage = data.Percentage,
                mention = data.Mention,
                isReussi = data.Percentage >= SeuilReussite,
                nbQuestions = data.Questions.Count,
                nbCorrectes = data.Questions.Count(q => q.Score >= q.MaxScore),
                Created = submittedAt,
                Modified = submittedAt,
                questions = data.Questions
                    .Select(q => new ResultatQuestion
                    {
                        questionId = q.QuestionId,
                        ordre = q.Ordre,
                        questionTexte = q.QuestionText,
                        questionType = q.QuestionType,
                        categorieId = q.CategorieId,
                        score = q.Score,
                        scoreMax = q.MaxScore,
                        isCorrecte = q.Score >= q.MaxScore,
                        isRepondue = q.IsAnswered,
                        reponseDonnee = q.GivenAnswer,
                        sortieObtenue = q.IsCodeQuestion ? q.ActualOutput : null,
                        Created = submittedAt,
                        Modified = submittedAt
                    })
                    .ToList()
            };

            await _dbContext.resultats.AddAsync(resultat);
            await _dbContext.SaveChangesAsync();
            return resultat;
        }

        public async Task<List<ResultatDTO>> ListByUser(string userId)
        {
            var userGuid = new Guid(userId);
            return await ToDTO(_dbContext.resultats.Where(r => r.userid == userGuid));
        }

        public async Task<List<ResultatDTO>> ListByQuiz(string quizId)
        {
            var quizGuid = new Guid(quizId);
            return await ToDTO(_dbContext.resultats.Where(r => r.quizid == quizGuid));
        }

        public async Task<List<ResultatDTO>> ListAll()
            => await ToDTO(_dbContext.resultats);

        public async Task<ResultatDetailDTO?> GetDetail(string resultatId)
        {
            var resultatGuid = new Guid(resultatId);

            var resultat = (await ToDTO(_dbContext.resultats.Where(r => r.Id == resultatGuid))).SingleOrDefault();
            if (resultat == null)
                return null;

            var questions = await _dbContext.resultatsQuestions
                .Where(q => q.resultatId == resultatGuid)
                .OrderBy(q => q.ordre)
                .Select(q => new ResultatQuestionDTO
                {
                    Ordre = q.ordre,
                    QuestionTexte = q.questionTexte,
                    QuestionType = q.questionType,
                    Score = q.score,
                    ScoreMax = q.scoreMax,
                    IsCorrecte = q.isCorrecte,
                    IsRepondue = q.isRepondue,
                    ReponseDonnee = q.reponseDonnee,
                    SortieObtenue = q.sortieObtenue
                })
                .ToListAsync();

            return new ResultatDetailDTO { Resultat = resultat, Questions = questions };
        }

        private static async Task<List<ResultatDTO>> ToDTO(IQueryable<Resultat> query)
            => await query
                .OrderByDescending(r => r.submittedAt)
                .Select(r => new ResultatDTO
                {
                    Id = r.Id.ToString(),
                    QuizId = r.quizid.HasValue ? r.quizid.Value.ToString() : null,
                    QuizTitre = r.quizTitre,
                    UserId = r.userid.ToString(),
                    SessionId = r.sessionId.ToString(),
                    Tentative = r.tentative,
                    StartedAt = r.startedAt,
                    SubmittedAt = r.submittedAt,
                    DureeSecondes = r.dureeSecondes,
                    IsAutoSubmitted = r.isAutoSubmitted,
                    Score = r.score,
                    ScoreMax = r.scoreMax,
                    Pourcentage = r.pourcentage,
                    Mention = r.mention,
                    IsReussi = r.isReussi,
                    NbQuestions = r.nbQuestions,
                    NbCorrectes = r.nbCorrectes
                })
                .ToListAsync();
    }
}
