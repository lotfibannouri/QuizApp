using QuizApp.Entities.Conception_Entities.DTO.ResultatDTO;

namespace Authentication.web.Services
{
    public interface IResultatService
    {
        Task<List<ResultatDTO>> ListAll();

        Task<ResultatDetailDTO?> GetDetail(string resultatId);
    }
}
