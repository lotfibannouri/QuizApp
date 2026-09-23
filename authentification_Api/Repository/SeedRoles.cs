using authentification_Api.Models;
using Microsoft.AspNetCore.Identity;

namespace authentification_Api.Repository
{
    public static class SeedRoles
    {
        public static async Task SeedAsync(RoleManager<Role> roleManager)
        {
            string[] roles =
            {
            "Admin",
            "User",
        };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                   var  result =await roleManager.CreateAsync(new Role(role));
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine(error.Description);
                        }
                    }
                }
            }
        }

        public static async Task SeedAdminAsync(
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
        {
            string email = "admin@test.com";
            string password = "Admin123!";

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new User
                {
                    nom="Admin1",
                    prenom="Admin",
                    UserName = email,
                    Email = email,
                    adresse = "Tunis"

                };

                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }


}
