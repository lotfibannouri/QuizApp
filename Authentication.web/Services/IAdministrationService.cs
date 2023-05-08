using Authentication.web.Model;
using Microsoft.AspNetCore.Identity;

namespace Authentication.web.Services
{
    public interface IAdministrationService
    {
        Task<HttpResponseMessage> CreateUserAsync(User user, string password);
        Task<Response> UpdateUserAsync(string id, SignUpModel model);
        Task<Response> DeleteUserAsync(string id);
        Task<IEnumerable<User>> GetUsers();
        Task<HttpResponseMessage?> GetUserByName(string name);
        Task<HttpResponseMessage?> GetUserById(string id);
    }
}
