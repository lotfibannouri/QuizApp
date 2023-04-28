using Authentication.web.Model;
using System.Net.Http;
using System.Reflection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace Authentication.web.Services
{
    public class AdministrationService : IAdministrationService
    {
        private readonly HttpClient _httpClient;
        public AdministrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public Task<HttpResponseMessage> CreateUserAsync(User user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> DeleteUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> GetUserById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage?> GetUserByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            int index = 0;
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Administration/ListUsers");
            IEnumerable<User?> Users = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<User?>>();
            httpResponseMessage = await _httpClient.GetAsync("/api/Administration/ListUsersRoles");
            IEnumerable<UserRole?> Usersroles = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<UserRole?>>();
            foreach (var item in Users)
            {
                item.rowNumber = ++index;
                var userrole = Usersroles.FirstOrDefault(x => x.userId == item.id);
                if (userrole != null)
                {   
                    item.role = userrole.roles;
                    
                }
            }
            return Users;
           
        }

        public Task<HttpResponseMessage> UpdateUserAsync(string id, SignUpModel model)
        {
            throw new NotImplementedException();
        }
    }
}
