using AutoMapper;
using ConceptionQuiz_Api.Models;
using Microsoft.EntityFrameworkCore;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Categorie_DTO;

namespace ConceptionQuiz_Api.Repository
{
    public class CategorieRepository : ICategorieRepository
    {
        private readonly ConceptionQuizDbContext _dbContext;
        private readonly IMapper _mapper;

        public CategorieRepository(ConceptionQuizDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Response> CreateCategorie(Categorie categorie)
        {
            await _dbContext.categories.AddAsync(categorie);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            if (rowsAffected > 0)
                return new Response(true, "création de catégorie réussie...");
            else
                return new Response(false, "création de catégorie a été échouée...");
        }

        public async Task<Response> UpdateCategorie(string id, Categorie categorie)
        {
            Categorie categorieRef = await GetCategorieById(id);
            categorieRef.titre = categorie.titre;
            categorieRef.description = categorie.description;
            _dbContext.categories.Update(categorieRef);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            if (rowsAffected > 0)
                return new Response(true, "modification de catégorie réussie...");
            else
                return new Response(false, "modification de catégorie a été échouée...");
        }

        public async Task<Response> DeleteCategorie(string id)
        {
            Categorie categorie = await GetCategorieById(id);
            _dbContext.categories.Remove(categorie);
            int rowsAffected = await _dbContext.SaveChangesAsync();
            if (rowsAffected > 0)
                return new Response(true, "suppression réussie");
            else
                return new Response(false, "suppression à été échouée");
        }

        public async Task<List<ListCategorieDTO>> ListCategorie()
        {
            List<Categorie> data = await _dbContext.categories.ToListAsync();
            List<ListCategorieDTO> listCategorie = new List<ListCategorieDTO>();
            foreach (Categorie categorie in data)
                listCategorie.Add(_mapper.Map<Categorie, ListCategorieDTO>(categorie));
            return listCategorie;
        }

        public async Task<Categorie> GetCategorieById(string id)
        {
            return await _dbContext.categories.SingleOrDefaultAsync(c => c.Id == new Guid(id));
        }
    }
}
