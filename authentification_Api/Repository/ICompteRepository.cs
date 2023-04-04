using authentification_Api.Models;
using Microsoft.AspNetCore.Identity;

namespace authentification_Api.Repository
{
    public interface ICompteRepository
    {
        Task<Response> SignUpAsync(SignUpModel model);
        Task<Response> LoginAsync(LoginModel model);
        Task<Response> logoutAsync();
    }
}
