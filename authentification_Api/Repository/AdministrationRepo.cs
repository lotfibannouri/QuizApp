using authentification_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace authentification_Api.Repository
{
        
    public class AdministrationRepo : IAdministrationRepository
    {
        #region attributs
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly QuizAppDbContext _dbContext;
        #endregion
        #region Constructors
        public AdministrationRepo(UserManager<User> userManager, RoleManager<Role> roleManager, QuizAppDbContext quizAppDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = quizAppDbContext;
        }

      
        #endregion
        #region methods users managed
        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
          return await _userManager.CreateAsync(user, password);

        }


        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            User? user = await GetUserById(id);
            return await _userManager.DeleteAsync(user);

        }


        public async Task<User?> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
          
        }

        public async Task<User?> GetUserByName(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public async Task<List<User>> GetUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }


        public async Task<IdentityResult> UpdateUserAsync(string id,SignUpModel model)
        {
            User? user = await GetUserById(id);
            if (user != null)
            {
                user.UserName = model.prenom + "." + model.nom;
                user.prenom = model.prenom;
                user.nom = model.nom;
                user.Email = model.login;
                user.adresse = model.adresse;
                user.PasswordHash = model.password;

                await _userManager.AddToRoleAsync(user, "User");
            }
            return await _userManager.UpdateAsync(user);
        }
        #endregion
        #region roles managed

        public Task<IdentityResult> CreateRoleAsync(Role role)
        {
           return _roleManager.CreateAsync(role);
        }
        public async Task<IdentityResult> DeleteRoleAsync(string id)
        {
            Role? role = await GetRoleById(id);
            return await _roleManager.DeleteAsync(role);
        }

        public async Task<Role?> GetRoleById(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }
        public async Task<Role?> GetRoleByName(string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }
        public async Task<List<Role>> GetRoles()
        {
           return await _dbContext.roles.ToListAsync();
        }
        public async Task<IdentityResult> UpdateRoleAsync(string id, ViewModelRole model)
        {
            Role? role = await GetRoleById(id);
            role.Name = model.name;

            return await _roleManager.UpdateAsync(role);
        }

        #endregion
        #region assign user to role
        public async Task<IdentityResult> AssignRole(string idUser, string idRole)
        {
            User? user = await GetUserById(idUser);
            Role? role = await GetRoleById(idRole);

            return await _userManager.AddToRoleAsync(user, role.Name);
        }
        #endregion
    }
}
