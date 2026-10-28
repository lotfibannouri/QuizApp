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
        public DbSet<Categorie> categories { get; set; }
        public DbSet<ProctoringEvent> proctoringEvents { get; set; }
        public DbSet<Resultat> resultats { get; set; }
        public DbSet<ResultatQuestion> resultatsQuestions { get; set; }

        public ConceptionQuizDbContext(DbContextOptions<ConceptionQuizDbContext> options) : base(options)
        {

        }

        // La connexion est configurée dans Program.cs à partir de ConnectionStrings:DefaultConnection.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuizUser>()
                .HasKey(qul => new { qul.quizid, qul.userid });

            modelBuilder.Entity<QuizUser>()
                .HasOne(qul => qul.quiz)
                .WithMany()
                .HasForeignKey(qul => qul.quizid);

            // Résultats : conservés à la suppression d'un quiz, d'une catégorie ou d'une
            // question (référence remise à NULL, les valeurs copiées restent).
            modelBuilder.Entity<Resultat>(resultat =>
            {
                resultat.HasOne(r => r.quiz)
                    .WithMany()
                    .HasForeignKey(r => r.quizid)
                    .OnDelete(DeleteBehavior.SetNull);

                resultat.HasOne(r => r.categorie)
                    .WithMany()
                    .HasForeignKey(r => r.categorieId)
                    .OnDelete(DeleteBehavior.SetNull);

                resultat.Property(r => r.mention).HasMaxLength(20);

                // Un passage ne peut être enregistré qu'une fois.
                resultat.HasIndex(r => r.sessionId).IsUnique();
                resultat.HasIndex(r => new { r.quizid, r.submittedAt });
                resultat.HasIndex(r => new { r.userid, r.submittedAt });
            });

            modelBuilder.Entity<ResultatQuestion>(detail =>
            {
                detail.HasOne(d => d.resultat)
                    .WithMany(r => r.questions)
                    .HasForeignKey(d => d.resultatId)
                    .OnDelete(DeleteBehavior.Cascade);

                detail.HasOne(d => d.question)
                    .WithMany()
                    .HasForeignKey(d => d.questionId)
                    .OnDelete(DeleteBehavior.SetNull);

                detail.Property(d => d.questionType).HasMaxLength(50);
            });
        }
    }
}
