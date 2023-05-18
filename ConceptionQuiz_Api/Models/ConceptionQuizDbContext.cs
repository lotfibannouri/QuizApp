using Microsoft.EntityFrameworkCore;

namespace ConceptionQuiz_Api.Models
{
    public class ConceptionQuizDbContext : DbContext
    {
        public DbSet<Question> questions { get; set; }
        public DbSet<Quiz> quiz { get; set; }

        public ConceptionQuizDbContext(DbContextOptions<ConceptionQuizDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder opbuilder)
        {
            opbuilder.UseSqlServer("Data Source=LAPTOP-GR853ENN\\SQL19;Initial Catalog=QuizConceptionDB;Integrated Security=True;Encrypt=False");
        }
    }
}
