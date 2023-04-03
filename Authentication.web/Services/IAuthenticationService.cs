using Authentication.web.Model;

namespace Authentication.web.Services
{
    public interface IAuthenticationService
    {
        Task<HttpResponseMessage> SignUpAsync(SignUpModel model);
        Task<HttpResponseMessage> LoginAsync(LoginModel model);
        Task<HttpResponseMessage> logoutAsync();
    }
}
