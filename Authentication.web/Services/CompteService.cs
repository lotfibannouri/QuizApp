using Authentication.web.Model;
using System.Net.Http.Json;

namespace Authentication.web.Services
{
    public class CompteService : ICompteService
    {
        private readonly HttpClient _httpClient;
        public CompteService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }
        public async Task<Response> LoginAsync(LoginModel model)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Compte/auth", model);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public Task<HttpResponseMessage> logoutAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> SignUpAsync(SignUpModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Compte/signUp", model);
            return response;
        }
    }
}
