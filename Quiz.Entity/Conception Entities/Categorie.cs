using QuizApp.Entities.Base_Entities;

namespace QuizApp.Entities.Conception_Entities
{
    public class Categorie : BaseEntity
    {
        public string titre { get; set; }
        public string? description { get; set; }
    }
}
