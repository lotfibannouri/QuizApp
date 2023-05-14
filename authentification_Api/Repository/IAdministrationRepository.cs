using authentification_Api.Models;
using Microsoft.AspNetCore.Identity;

namespace authentification_Api.Repository
{
    public interface IAdministrationRepository
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<IdentityResult> UpdateUserAsync(string id, SignUpModel model);
        Task<IdentityResult> DeleteUserAsync(string id);
        Task<List<User>> GetUsers();
        Task<User?> GetUserByName(string name);
        Task<User?> GetUserById(string id);
        Task<List<IdentityUserRole<string>>> GetUsersRoles();
        Task<IdentityResult> CreateRoleAsync(Role role);
        Task<IdentityResult> UpdateRoleAsync(string id, ViewModelRole model);
        Task<IdentityResult> DeleteRoleAsync(string id);
        Task<List<Role>> GetRoles();
        Task<Role?> GetRoleByName(string name);
        Task<Role?> GetRoleById(string id);
        Task<IdentityResult> AssignRole(string idUser, string idRole);
        Task<IdentityResult> ClearRoles(string idUser, List<string> roles);
    }
}
