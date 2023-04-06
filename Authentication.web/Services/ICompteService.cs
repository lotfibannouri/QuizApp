using Authentication.web.Model;

namespace Authentication.web.Services
{
    public interface ICompteService
    {
        Task<HttpResponseMessage> SignUpAsync(SignUpModel model);
        Task<Response> LoginAsync(LoginModel model);
        Task<HttpResponseMessage> logoutAsync();
    }
}
