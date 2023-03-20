using System.ComponentModel.DataAnnotations;

namespace authentification_Api.Models
{
    public class LoginModel
    {

        [Required,EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }

    }
}
