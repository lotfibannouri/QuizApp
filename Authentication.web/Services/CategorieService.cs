using Authentication.web.Model;
using AutoMapper;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.Categorie_DTO;
using System.Net.Http.Json;

namespace Authentication.web.Services
{
    public class CategorieService : ICategorieService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public CategorieService(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public async Task<Response> CreateCategorie(CreationCategorieDTO model)
        {
            Categorie categorie = _mapper.Map<CreationCategorieDTO, Categorie>(model);
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Categorie/AddCategorie", categorie);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public async Task<Response> UpdateCategorie(string id, UpdateCategorieDTO model)
        {
            Categorie categorie = _mapper.Map<UpdateCategorieDTO, Categorie>(model);
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Categorie/UpdateCategorie?id=" + id, categorie);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public async Task<Response> DeleteCategorie(string id)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("/api/Categorie/DeleteCategorie?id=" + id, null);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public async Task<List<ListCategorieDTO>> ListeCategorie()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Categorie/ListCategorie");
            IEnumerable<ListCategorieDTO>? listCategorie = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<ListCategorieDTO>>();
            return listCategorie.ToList();
        }
    }
}
