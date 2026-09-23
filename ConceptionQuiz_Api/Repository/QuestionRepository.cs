using AutoMapper;
using ConceptionQuiz_Api.Models;
using Microsoft.EntityFrameworkCore;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Http;

namespace ConceptionQuiz_Api.Repository
{
    public class QuestionRepository : IQuestionRepository
    {


        private readonly ConceptionQuizDbContext _dbContext;
        private readonly IMapper _mapper;
       

        public QuestionRepository(ConceptionQuizDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            
        }
        public async Task<Response> CreateQuestion(Question question)
        {
            await _dbContext.questions.AddAsync(question);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            if (rowsAffected > 0)
            {
                return new Response(true, "création de Question réussie...");

            }
            else
                return new Response(false, "création de Question a été échoué...");

        }

        public async Task<Response> DeleteQuestion(string id)
        {
            // Une question déjà assignée à un quiz ne doit jamais être supprimée :
            // on charge d'abord ses rattachements pour pouvoir refuser avant toute suppression.
            Question question = await _dbContext.questions
                .Include(q => q.quiz)
                .SingleOrDefaultAsync(q => q.Id == new Guid(id));

            if (question == null)
                return new Response(false, "question introuvable...");

            if (question.quiz != null && question.quiz.Any())
                return new Response(false, $"Cette question est utilisée dans {question.quiz.Count} quiz : elle ne peut pas être supprimée. Détachez-la d'abord des quiz concernés.");

            _dbContext.questions.Remove(question);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            if (rowsAffected > 0)
                return new Response(true, "suppression réussie");
            else
                return new Response(false, "suppression à été échouée");
        }

        public async Task<Question> GetQuestionById(string id)
        {
            return await _dbContext.questions
                .Include(p=>p.propositions).Include( p => p.reponses).Include(p => p.categorie)
                .SingleOrDefaultAsync(q => q.Id == new Guid(id));
        }

        public Task<Question> GetQuestionByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Question>>? ListQuestion()
        {

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            return await _dbContext.questions
                .Include(p => p.categorie)
                .ToListAsync();
        }

        public async Task<Response> UpdateQuestion(string id, Question Question)
        {
            Question questionRef = await _dbContext.questions
                .Include(q => q.propositions)
                .Include(q => q.reponses)
                .SingleOrDefaultAsync(q => q.Id == new Guid(id));

            if (questionRef == null)
                return new Response(false, "question introuvable...");

            questionRef.questionText = Question.questionText;
            questionRef.description = Question.description;
            questionRef.type = Question.type;
            questionRef.note = Question.note;
            questionRef.categorieId = Question.categorieId;

            // Les enfants sont remplacés en bloc : les entités reçues du client ont des Id vides,
            // on ne peut pas les rattacher telles quelles sans conflit de tracking EF.
            if (questionRef.propositions != null && questionRef.propositions.Any())
                _dbContext.propositions.RemoveRange(questionRef.propositions);

            if (questionRef.reponses != null && questionRef.reponses.Any())
                _dbContext.reponses.RemoveRange(questionRef.reponses);

            if (Question.propositions != null)
            {
                foreach (var proposition in Question.propositions)
                {
                    await _dbContext.propositions.AddAsync(new Proposition
                    {
                        textProposition = proposition.textProposition,
                        questionId = questionRef.Id
                    });
                }
            }

            if (Question.reponses != null)
            {
                foreach (var reponse in Question.reponses)
                {
                    await _dbContext.reponses.AddAsync(new Reponse
                    {
                        Body = reponse.Body,
                        IsRawAnswer = reponse.IsRawAnswer,
                        IsAnswer = reponse.IsAnswer,
                        output = reponse.output,
                        Language = reponse.Language,
                        QuestionId = questionRef.Id
                    });
                }
            }

            _dbContext.questions.Update(questionRef);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            if (rowsAffected > 0)
                return new Response(true, "modification de question réussie...");
            else
                return new Response(false, "modification de question a été échouée...");
        }


        public async Task<List<Question>> GetQuestionsByQuizId(string quizId)
        {
            Quiz quiz = await _dbContext.quiz
                              .Include(q => q.questions)
                              .FirstOrDefaultAsync(q => q.Id == new Guid(quizId));
            List<Question> questions = new List<Question>();
            if (quiz != null)
            {
                var ids = quiz.questions.Select(x => x.Id.ToString());
                foreach (var item in ids)
                {
                    var Question = await this.GetQuestionById(item);
                    if(Question!= null)  
                        questions.Add(Question);
                }
              
                //List<ListQuestionDTO> questionsDTO = new List<ListQuestionDTO>();
                //foreach (var question in questions)
                //{ var map = _mapper.Map<Question, ListQuestionDTO>(question); // je suis obligé d'utilisé le mapping dans le backend pour éviter
                //                                                              // la boucle infine des objets (l'entité question posséde une liste de quiz
                //                                                              //et l'entité quiz posséde aussi une liste de questions ).
                //    questionsDTO.Add(map);


                //}
                return questions;
            }

            return new List<Question>();
        }


     
    }
}
