using QuizApp.Entities.Conception_Entities.DTO.ResultatDTO;
using System.Net.Http.Json;

namespace Authentication.web.Services
{
    public class ResultatService : IResultatService
    {
        private readonly HttpClient _httpClient;

        public ResultatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultatDTO>> ListAll()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Resultat/ListAll");
            List<ResultatDTO> resultats = await httpResponseMessage.Content.ReadFromJsonAsync<List<ResultatDTO>>();
            return resultats ?? new List<ResultatDTO>();
        }

        public async Task<ResultatDetailDTO?> GetDetail(string resultatId)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Resultat/GetDetail?resultatId=" + resultatId);
            if (!httpResponseMessage.IsSuccessStatusCode)
                return null;

            return await httpResponseMessage.Content.ReadFromJsonAsync<ResultatDetailDTO>();
        }
    }
}
