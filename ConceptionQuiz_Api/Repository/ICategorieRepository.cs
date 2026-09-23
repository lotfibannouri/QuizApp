using ConceptionQuiz_Api.Models;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Categorie_DTO;

namespace ConceptionQuiz_Api.Repository
{
    public interface ICategorieRepository
    {
        Task<Response> CreateCategorie(Categorie categorie);
        Task<Response> UpdateCategorie(string id, Categorie categorie);
        Task<Response> DeleteCategorie(string id);
        Task<List<ListCategorieDTO>> ListCategorie();
        Task<Categorie> GetCategorieById(string id);
    }
}
