using AutoMapper;
using ConceptionQuiz_Api.Models;
using Microsoft.AspNetCore.Identity;
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
        public async Task<Response> CreateQuiz(CreationQuizDTO value)
        {
            Quiz quiz = _mapper.Map<CreationQuizDTO,Quiz>(value);
            await _dbContext.quiz.AddAsync(quiz);   
            int rowsAffected = await _dbContext.SaveChangesAsync();
            if(rowsAffected > 0)
            {
                return new Response(true, "création de Quiz réussie...");

            }
            else
                return new Response(false, "création de Quiz a été échoué...");


        }

        public async Task<bool> DeleteQuiz(string id)
        {
            Quiz? quiz = await GetQuizById(id);
            _dbContext.quiz.Remove(quiz);
            int rowsAffected = await _dbContext.SaveChangesAsync();

            return rowsAffected > 0;
        }

        public async Task<Quiz> GetQuizById(string id)
        {
            return await _dbContext.quiz.FindAsync(id);
        }

        public Task<Quiz> GetQuizByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ListQuizDTO>>? ListQuiz()
        {            
            List<Quiz>? data = await _dbContext.quiz.ToListAsync();
            List<ListQuizDTO> listQuiz = new List<ListQuizDTO>();
            if(data!= null)
            { 
            foreach (Quiz quiz in data)
                listQuiz.Add(_mapper.Map<Quiz, ListQuizDTO>(quiz));
            }
            return listQuiz;
        }


        public Task<bool> UpdateQuiz(string id, Quiz quiz)
        {
            throw new NotImplementedException();
        }
        #endregion
       

        public async Task<bool> AddToQuiz(string Idquestion, string Idquiz)
        {
            Quiz quizref = await this.GetQuizById(Idquiz);
            Question questionref =await _questionRepository.GetQuestionById(Idquestion);
           
            quizref.questions.Add(questionref);
            _dbContext.quiz.Update(quizref);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return (rowsAffected > 0);

        }
    }
}
