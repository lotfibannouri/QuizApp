using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace authentification_Api.Models
{
    public class User : IdentityUser
    {
        public string nom { get; set; }
        
        public string prenom { get; set; }

        
        public string adresse { get; set; }
        
        public DateTime dateCreation { get; set; }
    }
}
