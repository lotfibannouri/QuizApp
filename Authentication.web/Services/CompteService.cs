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

        public async Task logout()
        {
            await _localStorage.RemoveItemAsync("tokenAccess");
            ((AuthProvider)_authProvider).NotifyUserLogout();
            _httpClient.DefaultRequestHeaders.Authorization = null;
            
        }

        public async Task<Response> SignUpAsync(SignUpModel model)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Compte/signUp", model);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();

            return response!=null?response:new Response();
        }
    }
}
