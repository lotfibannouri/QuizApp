using AutoMapper;
using ConceptionQuiz_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace ConceptionQuiz_Api.Repository
{
    public class QuizRepository : IQuizRepository
    {
        #region properties
        private readonly ConceptionQuizDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IQuestionRepository _questionRepository;
        #endregion
        #region Constructors
        public QuizRepository(ConceptionQuizDbContext dbContext, IMapper mapper, IQuestionRepository questionRepository)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _questionRepository = questionRepository;
        }
        #endregion
        #region QuizMethods
        public async Task<Response> CreateQuiz(Quiz quiz)
        {
            await _dbContext.quiz.AddAsync(quiz);   
            int rowsAffected = await _dbContext.SaveChangesAsync();
            if(rowsAffected > 0)
            {
                return new Response(true, "création de Quiz réussie...");

            }
            else
                return new Response(false, "création de Quiz a été échoué...");


        }

        public async Task<Response> DeleteQuiz(string idquiz)
        {
            // La suppression effacerait aussi les assignations (cascade sur QuizUser).
            var assigned = await AssignedRefusal(idquiz);
            if (assigned != null)
                return assigned;

            Quiz quiz = await GetQuizById(idquiz);
            _dbContext.quiz.Remove(quiz);
            int rowsAffected = await _dbContext.SaveChangesAsync();

            if (rowsAffected > 0)
                return new Response(true, " suppression réussie ");

            else

                return new Response(true, " suppression à été échouée ");

        }

        public async Task<Quiz> GetQuizById(string id)
        {
            return await _dbContext.quiz.
                 Include(q => q.questions)
                 .Include(q => q.categorie)

                 .SingleOrDefaultAsync(q => q.Id == new Guid(id));

        }

        public Task<Quiz> GetQuizByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ListQuizDTO>>? ListQuiz()
        {            
            List<Quiz>? data = await _dbContext.quiz.Include(q => q.questions).Include(q => q.categorie).ToListAsync();
            // Une seule requête pour le nombre d'utilisateurs de tous les quiz.
            var assignedCounts = await _dbContext.QuizUser
                .GroupBy(qu => qu.quizid)
                .Select(g => new { QuizId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.QuizId, x => x.Count);

            List<ListQuizDTO> listQuiz = new List<ListQuizDTO>();
            if(data!= null)
            {
            foreach (Quiz quiz in data)
            {
                var dto = _mapper.Map<Quiz, ListQuizDTO>(quiz);
                dto.nbQuestionsRattachees = quiz.questions?.Count ?? 0;
                dto.nbUtilisateursAssignes = assignedCounts.TryGetValue(quiz.Id, out var count) ? count : 0;
                listQuiz.Add(dto);
            }
            }

            return listQuiz;
        }

        public async Task<List<QuizAssignmentDTO>> ListAssignments()
        {
            return await _dbContext.QuizUser
                .Select(qu => new QuizAssignmentDTO
                {
                    QuizId = qu.quizid.ToString(),
                    QuizTitre = qu.quiz.titre,
                    UserId = qu.userid.ToString(),
                    NbQuestionsPrevues = qu.quiz.nbr_questions,
                    NbQuestionsRattachees = qu.quiz.questions.Count
                })
                .ToListAsync();
        }

        public async Task<Response> UnbindQuizFromUser(string idQuiz, string idUser)
        {
            var assignment = await _dbContext.QuizUser
                .SingleOrDefaultAsync(qu => qu.quizid == new Guid(idQuiz) && qu.userid == new Guid(idUser));
            if (assignment == null)
                return new Response(false, "Ce quiz n'est pas assigné à cet utilisateur");

            _dbContext.QuizUser.Remove(assignment);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected > 0
                ? new Response(true, "Quiz détaché de l'utilisateur")
                : new Response(false, "Echec du détachement");
        }

        /// <summary>
        /// Refus commun à toute modification d'un quiz déjà assigné : les étudiants
        /// concernés passeraient un test différent de celui qui leur a été attribué.
        /// Renvoie null si le quiz n'est assigné à personne.
        /// </summary>
        public async Task<Response?> AssignedRefusal(string idQuiz)
        {
            var quizGuid = new Guid(idQuiz);
            int count = await _dbContext.QuizUser.CountAsync(qu => qu.quizid == quizGuid);
            return count == 0
                ? null
                : new Response(false, $"Ce quiz est assigné à {count} utilisateur(s) : détachez-le avant de pouvoir le modifier.");
        }
        public async Task<List<QuizUser>>? ListQuizUser()
        {
            List<QuizUser>? data = await _dbContext.QuizUser.ToListAsync();
            return data;
        }


        public Task<bool> UpdateQuiz(string id, Quiz quiz)
        {
            throw new NotImplementedException();
        }
        #endregion
       

        public async Task<Response> BindQuizToQuestion(string Idquestion, string Idquiz)
        {
            var assigned = await AssignedRefusal(Idquiz);
            if (assigned != null)
                return assigned;

            Quiz quizref = await this.GetQuizById(Idquiz);
            Question questionref =await _questionRepository.GetQuestionById(Idquestion);
            if(quizref.questions.Contains(questionref))
                return new Response(false, "Cette question existe dèja");
            quizref.questions.Add(questionref);
            _dbContext.quiz.Update(quizref);
            int rowsAffected = await _dbContext.SaveChangesAsync();
           if(rowsAffected> 0)
            {
                return new Response(true, "Question(s) affectée(s) avec succès ");
            }
           else
                return new Response(false, "Echec");

        }

        public async Task<Response> UnbindQuizFromQuestion(string idQuiz, string idQuestion)
        {
            var assigned = await AssignedRefusal(idQuiz);
            if (assigned != null)
                return assigned;

            Quiz quizref = await this.GetQuizById(idQuiz);
            Question questionref = await _questionRepository.GetQuestionById(idQuestion);
            if (!quizref.questions.Contains(questionref))
                return new Response(false, "Cette question n'existe pas");
            quizref.questions.Remove(questionref);
            _dbContext.quiz.Update(quizref);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            if (rowsAffected > 0)
            {
                return new Response(true, "Question(s) Détachée(s) avec succès ");
            }
            else
                return new Response(false, "Echec");
        }

        public async Task<Response> BindQuizToUser(QuizUser quizUser)
        {
            // Seul un quiz complet (questions rattachées = nombre prévu) peut être assigné.
            var quiz = await _dbContext.quiz
                .Include(q => q.questions)
                .SingleOrDefaultAsync(q => q.Id == quizUser.quizid);
            if (quiz == null)
                return new Response(false, "quiz introuvable...");

            int rattachees = quiz.questions?.Count ?? 0;
            if (rattachees != quiz.nbr_questions)
                return new Response(false, $"Quiz incomplet ({rattachees}/{quiz.nbr_questions} questions) : il ne peut pas être assigné.");

            // Sans ce contrôle, une seconde assignation violerait la clé (quizid, userid).
            if (await _dbContext.QuizUser.AnyAsync(qu => qu.quizid == quizUser.quizid && qu.userid == quizUser.userid))
                return new Response(false, "Ce quiz est déjà assigné à cet utilisateur");

            await _dbContext.QuizUser.AddAsync(quizUser);
            int rowsAffected = await _dbContext.SaveChangesAsync();
           
            if (rowsAffected > 0)
            {
                return new Response(true, "Quiz Binded...");

            }
            else
                return new Response(false, "Quiz not Binded...");

        }
       
    }
}
