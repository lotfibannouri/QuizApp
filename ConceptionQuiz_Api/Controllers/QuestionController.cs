using ConceptionQuiz_Api.Models;
using ConceptionQuiz_Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<Response> CreateQuestion([FromBody] CreationQ_PropDTO question)
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
    }
}
