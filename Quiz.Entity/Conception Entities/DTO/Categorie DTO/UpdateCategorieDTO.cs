using QuizApp.Entities.Base_DTO;

namespace QuizApp.Entities.Conception_Entities.DTO.Categorie_DTO
{
    public class UpdateCategorieDTO : BaseUpdateDTO
    {
        public string titre { get; set; }
        public string? description { get; set; }
    }
}
