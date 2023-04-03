using System.ComponentModel.DataAnnotations;

namespace Authentication.web.Model
{
    public class LoginModel
    {
        
            [Required, EmailAddress]
            public string email { get; set; }
            [Required]
            public string password { get; set; }
        }
    
}
