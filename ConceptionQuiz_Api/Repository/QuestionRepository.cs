using AutoMapper;
using ConceptionQuiz_Api.Models;
using Microsoft.EntityFrameworkCore;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using System.Text.Json.Serialization;
using System.Text.Json;

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

        public Task<Response> DeleteQuestion(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Question> GetQuestionById(string id)
        {
            return await _dbContext.questions
                .Include(p=>p.propositions)
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
                .ToListAsync();
        }

        public Task<Response> UpdateQuestion(string id, Question Question)
        {
            throw new NotImplementedException();
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
