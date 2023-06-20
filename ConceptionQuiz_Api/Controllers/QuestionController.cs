using ConceptionQuiz_Api.Models;
using ConceptionQuiz_Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace ConceptionQuiz_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;
        public QuestionController(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }


        [HttpPost("AddQuestion")]
        public async Task<Response> CreateQuestion([FromBody] Question question)
        {
            try
            {
                var result = await _questionRepository.CreateQuestion(question);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }



        [HttpGet("ListQuestion")]
        public async Task<List<Question>> ListQuestion()
        {
            try
            {
                var result = await _questionRepository.ListQuestion();
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("GetQuestionsByQuizId")]
        public async Task<List<Question>> GetQuestionsByQuizId(string QuizId)
        {
            try
            {
                var result = await _questionRepository.GetQuestionsByQuizId(QuizId);
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        [HttpGet("GetQuestionsById")]
        public async Task<Question> GetQuestionsById(string QuestionId)
        {
            try
            {
                var result = await _questionRepository.GetQuestionById(QuestionId);
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
