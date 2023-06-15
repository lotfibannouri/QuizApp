using Authentication.web.Model;
using System.Net.Http;
using System.Reflection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;



namespace Authentication.web.Services
{
    public class AdministrationService : IAdministrationService
    {
        private readonly HttpClient _httpClient;
        public AdministrationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Response> AssignRoleAsync(string idUser, string role)
        {
            List<Roles> roles = await GetRoles();
            if(role == null) {
                return new Response(false, "Liste des roles est vide ...");
            }
            string idRole = roles.Find(x=>x.name == role).id;
            //Dictionary<string,string> parameters = new Dictionary<string,string>();
            //parameters.Add(idUser, idRole);
            //var content = new FormUrlEncodedContent(parameters);

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("/api/Administration/AssignRole?idUser="+ idUser+ "&"+ "idRole="+idRole,null);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
           
            return response;
        }

        public async Task<Response> clearRolesAsync(string idUser, List<string> roles)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Administration/ClearRoles?idUser=" + idUser , roles);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();

            return response;
        }
        public Task<HttpResponseMessage> CreateUserAsync(User user, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<Response> DeleteUserAsync(string id)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.DeleteAsync("/api/Administration/deleteUser?id="+id)   ;
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>() ;
            return response;

        }

        public async Task<User?> GetUserById(string id)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Administration/userById?id=" + id);
            User response = await httpResponseMessage.Content.ReadFromJsonAsync<User>();
            return response;
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

        public async Task<Response> UpdateUserAsync(string id, SignUpModel model)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Administration/updateUser?id="+id, model);
            Response result = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return result;


        }


        public async Task<List<Roles>> GetRoles()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Administration/ListRoles");
            List<Roles> result = await httpResponseMessage.Content.ReadFromJsonAsync<List<Roles>>();
            return result;

        }
    }
}
