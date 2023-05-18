using ConceptionQuiz_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConceptionQuiz_Api.Repository
{
    public class QuizRepository : IQuizRepository
    {
        #region properties
        private readonly ConceptionQuizDbContext _dbContext;
        #endregion
        #region Constructors
        public QuizRepository(ConceptionQuizDbContext dbContext)
        {

            _dbContext = dbContext;

        }
        #endregion
        #region QuizMethods
        public async Task<bool> CreateQuiz(Quiz quiz)
        {
            
            await _dbContext.quiz.AddAsync(quiz);   
            int rowsAffected = await _dbContext.SaveChangesAsync();
            return rowsAffected > 0;


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

        public async Task<List<Quiz>> ListQuiz()
        {
            return await _dbContext.quiz.ToListAsync();
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
