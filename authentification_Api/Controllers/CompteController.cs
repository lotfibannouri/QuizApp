using authentification_Api.Models;
using authentification_Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace authentification_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompteController : ControllerBase
    {
        private readonly ICompteRepository _compteRepository ;
        public CompteController(ICompteRepository compteRepository)
        {
            _compteRepository = compteRepository ;
        }



        [HttpPost("auth")]
        public async Task<IActionResult> LogIn([FromBody]LoginModel model )
        {
            var result = await _compteRepository.LoginAsync(model);

            if(result.status)
                return Ok(result);
            else
               return NotFound(result);
        }


        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp([FromBody]SignUpModel model)
        {
            var result = await _compteRepository.SignUpAsync(model);
            if (result.status)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
          var result =  await _compteRepository.logoutAsync();
          if (result.status)
                return Ok(result);
          else return BadRequest(result);
            
        }
    }
}
