using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Net.Http.Headers;
using Authentication.web.utility;

namespace Authentication.web.AuthProviders
{
    public class AuthProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationState _anonymous;

        public AuthProvider(HttpClient http,ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async  Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync("tokenAccess");
            if (string.IsNullOrEmpty(token))
            {
                return _anonymous;
            }
            _http.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new ClaimsPrincipal(
                new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "JwtAuthType")));
             
        }

        public void NotifyUserAuthentication(string username)
        {
            var authUser = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Name, username) }, "JwtAuthType"
                )); 
            var authState = Task.FromResult(new AuthenticationState(authUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState); 
        }
    }
}
