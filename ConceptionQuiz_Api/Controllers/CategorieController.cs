using ConceptionQuiz_Api.Models;
using ConceptionQuiz_Api.Repository;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Entities.Conception_Entities;

namespace ConceptionQuiz_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategorieController : ControllerBase
    {
        private readonly ICategorieRepository _categorieRepository;

        public CategorieController(ICategorieRepository categorieRepository)
        {
            _categorieRepository = categorieRepository;
        }

        [HttpPost("AddCategorie")]
        public async Task<Response> CreateCategorie([FromBody] Categorie categorie)
        {
            try
            {
                var result = await _categorieRepository.CreateCategorie(categorie);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpGet("ListCategorie")]
        public async Task<IActionResult> GetCategorie()
        {
            try
            {
                var result = await _categorieRepository.ListCategorie();
                if (result != null)
                    return Ok(result);
                else
                    return NotFound("liste des catégories est vide ");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("UpdateCategorie")]
        public async Task<Response> UpdateCategorie(string id, [FromBody] Categorie categorie)
        {
            try
            {
                var result = await _categorieRepository.UpdateCategorie(id, categorie);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        [HttpPost("DeleteCategorie")]
        public async Task<Response> DeleteCategorie(string id)
        {
            try
            {
                var result = await _categorieRepository.DeleteCategorie(id);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
