using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities;
using System.Net.Http;
using Authentication.web.Model;
using System.Net.Http.Json;

namespace Authentication.web.Services
{
    public class JdoodleService : IJdoodleService
    {
        private readonly HttpClient _httpClient;
        public JdoodleService(HttpClient httpClient)
        {
            _httpClient = httpClient; 
        }

        public async Task<string> GetOutputCodeQuestion(string code)
        {
            AttemptCode cq = new AttemptCode();
            cq.script = code;
            cq.versionIndex = "4";
            cq.language = "csharp";
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("https://api.jdoodle.com/", cq);
            OutputCode response = await httpResponseMessage.Content.ReadFromJsonAsync<OutputCode>();

            return response.output;
        }

       
    }
}
