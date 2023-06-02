using AutoMapper;
using ConceptionQuiz_Api.Models;
using Microsoft.EntityFrameworkCore;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

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

        public Task<Question> GetQuestionById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Question> GetQuestionByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<List<ListQuizDTO>> ListQuestion()
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateQuestion(string id, Question Question)
        {
            throw new NotImplementedException();
        }
    }
}
