using Authentication.Web.Client.Model;

namespace Authentication.Web.Client.Service
{
    public interface IAuthentificationService
    {
        Task<HttpResponseMessage> SignUpAsync(SignUpModel model);
        Task<HttpResponseMessage> LoginAsync(LoginModel model);
        Task<HttpResponseMessage> logoutAsync();
    }
}
