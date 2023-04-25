using Authentication.web.AuthProviders;
using Authentication.web.Model;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using static System.Net.WebRequestMethods;

namespace Authentication.web.Services
{
    public class CompteService : ICompteService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authProvider;

        public CompteService(HttpClient httpClient,ILocalStorageService localStorage, AuthenticationStateProvider authProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authProvider = authProvider;
        }
        public async Task<Response> LoginAsync(LoginModel model)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Compte/auth", model);
            Response? response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            if (httpResponseMessage.IsSuccessStatusCode && response.content != null)
            {
                await _localStorage.SetItemAsync("tokenAccess",response.content);
                ((AuthProvider)_authProvider).NotifyUserAuthentication(response.content);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", response.content);
            }
            return response;
        }

        public async Task<Response> logoutAsync()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Compte/auth");
            Response? response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();

            await _localStorage.RemoveItemAsync("tokenAccess");
            ((AuthProvider)_authProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return response;
        }

        public async Task<HttpResponseMessage> SignUpAsync(SignUpModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Compte/signUp", model);
            return response;
        }
    }
}
