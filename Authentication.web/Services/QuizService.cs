using Authentication.web.Model;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
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

 

        public async Task<Response> CreateQuiz(CreationQuizDTO quiz)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Quiz/AddQuiz", quiz);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public async Task<List<ListQuizDTO>> ListeQuiz()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Quiz/ListQuiz");
            IEnumerable<ListQuizDTO>? listQuiz = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<ListQuizDTO>>();
            return listQuiz.ToList();

           
        }
    }
}
