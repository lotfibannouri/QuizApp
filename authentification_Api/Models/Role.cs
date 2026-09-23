using Microsoft.AspNetCore.Identity;

namespace authentification_Api.Models
{
    public class Role : IdentityRole
    {
        public Role(string role)
        {
            Name = role;
        }
        public Role()
        {

        }
    }
}
