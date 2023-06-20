using Authentication.web.Model;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using System.Net.Http.Json;
using static MudBlazor.Colors;
using Authentication.web.utility;

namespace Authentication.web.Services
{
    public class QuestionService : IQuestionService
    {

        private readonly HttpClient _httpClient;

        private readonly IMapper _mapper;
        public QuestionService(HttpClient httpClient, IMapper mapper)
        {
         _httpClient = httpClient;
            _mapper = mapper;
        }
        public async Task<Response> CreateQuestion(CreationQ_PropDTO value)
        {
            Question question = _mapper.Map<CreationQ_PropDTO, Question>(value);
            question.quiz = new List<Quiz>();
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Question/AddQuestion", question);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public async Task<List<ListQuestionDTO>> GetQuestions()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Question/ListQuestion");
            List<Question> response = await httpResponseMessage.Content.ReadFromJsonAsync<List<Question>>();
            List<ListQuestionDTO> questions = new List<ListQuestionDTO>();
            foreach(var question in response)
                questions.Add( _mapper.Map<Question, ListQuestionDTO>(question));

            return questions;

        }

        public async Task<List<ListQuestionDTO>> GetQuestionsByQuizId(string quizId)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Question/GetQuestionsByQuizId?QuizId="+quizId);
            List<Question> response = await httpResponseMessage.Content.ReadFromJsonAsync<List<Question>>();
            List<ListQuestionDTO> questions = new List<ListQuestionDTO>();
            foreach (var question in response)
                questions.Add(_mapper.Map<Question, ListQuestionDTO>(question));

            return questions;
        }
        public async Task<ListQuestionDTO> GetQuestionsById(string questionId)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Question/GetQuestionsById?QuestionId=" + questionId);
            Question response = await httpResponseMessage.Content.ReadFromJsonAsync<Question>();
            ListQuestionDTO question= _mapper.Map<Question, ListQuestionDTO>(response);
            return question;
        }
    }
}
