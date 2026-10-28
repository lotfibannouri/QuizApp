using ConceptionQuiz_Api.Metier;
using ConceptionQuiz_Api.Models;
using ConceptionQuiz_Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using System.Linq;
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
        private readonly IQuizReport _quizReport;
        private readonly IConfiguration _configuration;
        public QuestionController(IQuestionRepository questionRepository,  IHttpClientFactory httpClientFactory, IQuizReport quizReport, IConfiguration configuration)
        {
            _questionRepository = questionRepository;
            _httpClient = httpClientFactory.CreateClient();
            _quizReport = quizReport;
            _configuration = configuration;
        }


        [HttpPost("AddQuestion")]
        public async Task<Response> CreateQuestion([FromBody] Question question, string? quizId = null)
        {
            try
            {
                var result = await _questionRepository.CreateQuestion(question, quizId);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }



        [HttpPost("UpdateQuestion")]
        public async Task<Response> UpdateQuestion(string id, [FromBody] Question question)
        {
            try
            {
                var result = await _questionRepository.UpdateQuestion(id, question);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }


        [HttpPost("DeleteQuestion")]
        public async Task<Response> DeleteQuestion(string id)
        {
            try
            {
                var result = await _questionRepository.DeleteQuestion(id);
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
        [HttpPost("CalculateTrueFalseScore")]
        public async Task<double> CalculateTrueFalseScore([FromBody] ScoreRequestDTO request)
        {
            try
            {
                return await _quizReport.CalculateTrueFalseScore(request);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("CalculateMultiChoiceScore")]
        public async Task<double> CalculateMultiChoiceScore([FromBody] ScoreRequestDTO request)
        {
            var question = await _questionRepository.GetQuestionById(request.QuestionId);
            if (question?.reponses == null)
                return 0;

            bool hasCorrectChecked = question.reponses.Any(mapped =>
                mapped.IsAnswer && request.Answers.Any(a => a.Body == mapped.Body && a.IsChecked));

            double score = 0;
            foreach (var mapped in question.reponses)
            {
                bool isChecked = request.Answers.Any(a => a.Body == mapped.Body && a.IsChecked);
                bool isMatch = mapped.IsAnswer == isChecked;

                if (hasCorrectChecked)
                    score += isMatch ? 0.25 : -0.25;
                else if (isChecked && !mapped.IsAnswer)
                    score -= 0.25;
            }

            return score;
        }

        [HttpPost("GetOutput")]
        public async Task<OutputCode> GetOutput([FromBody] AttemptCode cq)
        {
            // Les identifiants JDoodle restent côté serveur (user secrets en local,
            // paramètres JDoodle__ClientId / JDoodle__ClientSecret sur Azure).
            var json = JsonConvert.SerializeObject(new
            {
                clientId = _configuration["JDoodle:ClientId"],
                clientSecret = _configuration["JDoodle:ClientSecret"],
                cq.language,
                cq.script,
                cq.versionIndex
            });
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); 
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("https://api.jdoodle.com/v1/execute", stringContent);
            OutputCode response = await httpResponseMessage.Content.ReadFromJsonAsync<OutputCode>();
            return response;
        }

    }
}
