using Authentication.web.Model;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using System.Net.Http.Json;

namespace Authentication.web.Services
{
    public class QuestionService : IQuestionService
    {

        private readonly HttpClient _httpClient;

        public QuestionService(HttpClient httpClient)
        {
         _httpClient = httpClient;
        }
        public async Task<Response> CreateQuestion(CreationQ_PropDTO question)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Question/AddQuestion", question);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }
    }
}
