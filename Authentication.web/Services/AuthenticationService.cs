using Authentication.web.Model;
using System.Net.Http.Json;

namespace Authentication.web.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }
        public async Task<HttpResponseMessage> LoginAsync(LoginModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Compte/auth", model);
            return response;
        }

        public Task<HttpResponseMessage> logoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> SignUpAsync(SignUpModel model)
        {
            throw new NotImplementedException();
        }
    }
}
