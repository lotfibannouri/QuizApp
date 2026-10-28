using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace authentification_Api.Models
{
    public class QuizAppDbContext : IdentityDbContext<User>
    {
        public QuizAppDbContext(DbContextOptions<QuizAppDbContext> options) : base(options)
        {
            
        }


        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }

        // La connexion est configurée dans Program.cs à partir de ConnectionStrings:DefaultConnection.
    }
}
