using Authentification.Web.Model;

namespace Authentification.Web.Service
{
    public interface IAuthentificationService
    {
        Task<HttpResponseMessage> SignUpAsync(SignUpModel model);
        Task<HttpResponseMessage> LoginAsync(LoginModel model);
        Task<HttpResponseMessage> logoutAsync();
    }
}
