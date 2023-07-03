using ConceptionQuiz_Api.Models;
using ConceptionQuiz_Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using System.Net.Http;
using System.Text;

namespace ConceptionQuiz_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly HttpClient _httpClient;
        public QuestionController(IQuestionRepository questionRepository,  IHttpClientFactory httpClientFactory)
        {
            _questionRepository = questionRepository;
            _httpClient = httpClientFactory.CreateClient();
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
        [HttpPost("GetOutput")]
        public async Task<OutputCode> GetOutput([FromBody] AttemptCode cq)
        {
            var json =  JsonConvert.SerializeObject(cq);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); 
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("https://api.jdoodle.com/v1/execute", stringContent);
            OutputCode response = await httpResponseMessage.Content.ReadFromJsonAsync<OutputCode>();
            return response;
        }

    }
}
