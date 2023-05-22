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
        #endregion
        #region Constructors
        public QuizRepository(ConceptionQuizDbContext dbContext, IMapper mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;

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

        public async Task<List<ListQuizDTO>> ListQuiz()
        {
            
            List<Quiz> data = await _dbContext.quiz.ToListAsync();
            List<ListQuizDTO> listQuiz = new List<ListQuizDTO>();
            foreach (Quiz quiz in data)
                listQuiz.Add(_mapper.Map<Quiz, ListQuizDTO>(quiz));
            return listQuiz;
        }


        public Task<bool> UpdateQuiz(string id, Quiz quiz)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region QuestionMethods
        public async Task<bool> CreateQuestion(Question question)
        {
            await _dbContext.questions.AddAsync(question);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected > 0;

        }
        public async Task<bool> DeleteQuestion(string id)
        {
            Question? question = await GetQuestionById(id);
            _dbContext.questions.Remove(question);
            int rowsAffected = await _dbContext.SaveChangesAsync();

            return rowsAffected > 0;
        }
        public async Task<Question> GetQuestionById(string id)
        {
            return await _dbContext.questions.FindAsync(id);
        }

        public async Task<List<Question>> ListQuestions()
        {
            return await _dbContext.questions.ToListAsync();
        }
        #endregion

        public async Task<bool> AddToQuiz(string Idquestion, string Idquiz)
        {
            Quiz quizref = await this.GetQuizById(Idquiz);
            Question questionref =await GetQuestionById(Idquestion);
           
            quizref.questions.Add(questionref);
            _dbContext.quiz.Update(quizref);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return (rowsAffected > 0);

        }
    }
}
