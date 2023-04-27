using Authentication.web.Model;

namespace Authentication.web.Services
{
    public interface IAdministrationService
    {
        Task<HttpResponseMessage> CreateUserAsync(User user, string password);
        Task<HttpResponseMessage> UpdateUserAsync(string id, SignUpModel model);
        Task<HttpResponseMessage> DeleteUserAsync(string id);
        Task<IEnumerable<User>> GetUsers();
        Task<HttpResponseMessage?> GetUserByName(string name);
        Task<HttpResponseMessage?> GetUserById(string id);
    }
}
