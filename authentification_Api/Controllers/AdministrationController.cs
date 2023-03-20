using authentification_Api.Models;
using authentification_Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace authentification_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class AdministrationController : ControllerBase
    {
        #region attributs
        private readonly IAdministrationRepository _administrationRepository;
        #endregion

        #region Constructors
        public AdministrationController(IAdministrationRepository  administration) { 
                _administrationRepository = administration;        
        }
        #endregion

        #region methods users managed
        [HttpGet("ListUsers")]
        public async Task<IActionResult> GetUsers() 
        { 
            var result = await _administrationRepository.GetUsers();
            if(result!= null)
                return Ok(result);
            else
                return NotFound("la liste des utilisateurs est vide ");

        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> CreateUser([FromBody] SignUpModel newUser)
        {
            User user = new User()
            {
                UserName = newUser.prenom+"."+newUser.nom,
                prenom = newUser.prenom,
                nom = newUser.nom,
                Email = newUser.login,
                adresse = newUser.adresse,
                dateCreation = DateTime.Now

            };
           
            var result = await _administrationRepository.CreateUserAsync(user,newUser.password);

            if(result.Succeeded)
                return Ok(result);
            else
                return BadRequest(result);
            
        }

        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _administrationRepository.DeleteUserAsync(id);

            if (result.Succeeded)
                return Ok(result);
            else
                return BadRequest(result);

        }

        [HttpGet("userById")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _administrationRepository.GetUserById(id);
            if(result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("utilisateur n'existe pas ");
            }
        }

        [HttpGet("userByName")]
        public async Task<IActionResult> GetUserByName(string name)
        {
            var result = await _administrationRepository.GetUserByName(name);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("utilisateur n'existe pas ");
            }
        }

        [HttpPost("updateUser")]
        public async Task<IActionResult> UpdateUser(string id ,[FromBody] SignUpModel model)
        {
            var result = await _administrationRepository.UpdateUserAsync(id,model);
            if (result.Succeeded)
                return Ok(result);
            else
                return BadRequest(result);
        }
        #endregion

        #region methods roles manager
        [HttpPost("AddRole")]
        public async Task<IActionResult> CreateRole([FromBody] ViewModelRole model)
        {
            Role role = new Role()
            {
                Name = model.name

            };
            var result = await _administrationRepository.CreateRoleAsync(role);
            if(result.Succeeded)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("ListRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _administrationRepository.GetRoles();
            if (result != null)
                return Ok(result);
            else
                return NotFound("la liste des roles est vide ");

        }

        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRolde(string id)
        {
            var result = await _administrationRepository.DeleteRoleAsync(id);

            if (result.Succeeded)
                return Ok(result);
            else
                return BadRequest(result);

        }

        [HttpGet("roleById")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var result = await _administrationRepository.GetRoleById(id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("role n'existe pas ");
            }
        }

        [HttpGet("RoleByName")]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            var result = await _administrationRepository.GetRoleByName(name);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("role n'existe pas ");
            }
        }

        [HttpPost("updateRole")]
        public async Task<IActionResult> UpdateRole (string id, [FromBody] ViewModelRole model)
        {
            var result = await _administrationRepository.UpdateRoleAsync(id, model);
            if (result.Succeeded)
                return Ok(result);
            else
                return BadRequest(result);
        }

        #endregion
        #region Assign user to role
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignToRole(string idUser, string idRole)
        {
            var result = await _administrationRepository.AssignRole(idUser, idRole);
            if (result.Succeeded)
                return Ok(result);
            else
                return BadRequest(result);
        }
        #endregion
    }
}
