using Microsoft.EntityFrameworkCore;
using QuizApp.Entities.Conception_Entities;

namespace ConceptionQuiz_Api.Models
{
    public class ConceptionQuizDbContext : DbContext
    {
        public DbSet<Question> questions { get; set; }
        public DbSet<Quiz> quiz { get; set; }
        public DbSet<Proposition> propositions { get; set; }
        public DbSet<Reponse> reponses { get; set; }
        public DbSet<QuizUser> QuizUser { get; set; }

        public ConceptionQuizDbContext(DbContextOptions<ConceptionQuizDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder opbuilder)
        {
            opbuilder.UseSqlServer("Data Source=PC10\\SQLEXPRESS;Initial Catalog=QuizConceptionDB;Integrated Security=True;Encrypt=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuizUser>()
                .HasKey(qul => new { qul.quizid, qul.userid });

            modelBuilder.Entity<QuizUser>()
                .HasOne(qul => qul.quiz)
                .WithMany()
                .HasForeignKey(qul => qul.quizid);
        }
    }
}
