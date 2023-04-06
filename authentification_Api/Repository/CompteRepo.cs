﻿using authentification_Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace authentification_Api.Repository
{
    public class CompteRepo : ICompteRepository
    {
        public readonly UserManager<User> _userManager;
        public readonly SignInManager<User> _signInManager;
        public readonly IConfiguration _configuration;
        public readonly Response _response;

        public CompteRepo(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration,Response response)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _response = response;
        }
         
        public async Task<Response> LoginAsync(LoginModel model)
        {
            User? user = await _userManager.FindByEmailAsync(model.email);
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.password, false, false);

            if (!result.Succeeded)
            {
                _response.status = false;
                _response.content = "Email ou mot de passe incorrecte !";
                return _response;
            }
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role,"User")

            };
            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
                );

            _response.status = true;
            _response.content = new JwtSecurityTokenHandler().WriteToken(token);
            return _response;

            
        }

        public async Task<Response> logoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _response.status = true;
                _response.content = "utilisateur déconnecté";
                return _response;
            }
            catch 
            {
                _response.status = false;
                _response.content = "erreur de déconnexion";
                return _response;
            }
        }

        public async Task<Response> SignUpAsync(SignUpModel model)
        {
            var user = new User()
            {
                nom = model.nom,
                prenom = model.prenom,
                Email = model.login,
                adresse = model.adresse,
                UserName = model.prenom+"."+model.nom,

            };
            var result = await _userManager.CreateAsync(user, model.password);
            if(result.Succeeded)
            {
                _response.status = true;
                _response.content = "inscription avec succées";
                return _response;

            }
            else
            {
                _response.status = false;
                _response.content = "erreur d'inscription";
                return _response;
            }
        }

    }
}
