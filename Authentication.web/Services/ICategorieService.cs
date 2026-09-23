using Authentication.web.Model;
using QuizApp.Entities.Conception_Entities.DTO.Categorie_DTO;

namespace Authentication.web.Services
{
    public interface ICategorieService
    {
        Task<Response> CreateCategorie(CreationCategorieDTO categorie);
        Task<Response> UpdateCategorie(string id, UpdateCategorieDTO categorie);
        Task<Response> DeleteCategorie(string id);
        Task<List<ListCategorieDTO>> ListeCategorie();
    }
}
