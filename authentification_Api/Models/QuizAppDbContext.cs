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
     

        protected override void OnConfiguring(DbContextOptionsBuilder opbuilder)
        {
            opbuilder.UseSqlServer("Data Source=PC06\\SQLEXPRESS01;Initial Catalog=QuizAuthenticationDB;Integrated Security=True;Encrypt=False");
        }
    }
}
