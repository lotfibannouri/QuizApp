using Microsoft.EntityFrameworkCore;

namespace ConceptionQuiz_Api.Models
{
    public class ConceptionQuizDbContext : DbContext
    {

        public ConceptionQuizDbContext(DbContextOptions<ConceptionQuizDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder opbuilder)
        {
            opbuilder.UseSqlServer("Data Source=PC06\\SQLEXPRESS01;Initial Catalog=QuizConceptionDB;Integrated Security=True;Encrypt=False");
        }
    }
}
