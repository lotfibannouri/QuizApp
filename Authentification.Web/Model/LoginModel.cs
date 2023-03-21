using System.ComponentModel.DataAnnotations;

namespace Authentification.Web.Model
{
    public class LoginModel
    {
        [Required, EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
