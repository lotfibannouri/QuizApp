using Authentication.web.Model;
using System.Net.Http.Json;

namespace Authentication.web.Services
{
    public class QuizService : IQuizService
    {
        private readonly HttpClient _httpClient;
        public QuizService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

 

        public async Task<Response> CreateQuiz(Quiz quiz)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Quiz/AddQuiz", quiz);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }
    }
}
