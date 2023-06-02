using Authentication.web.Model;
using AutoMapper;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;
using System.Net.Http;
using System.Net.Http.Json;
using static MudBlazor.Colors;

namespace Authentication.web.Services
{
    public class QuizService : IQuizService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public QuizService(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }

 

        public async Task<Response> CreateQuiz(CreationQuizDTO model)
        {

            Quiz quiz = _mapper.Map<CreationQuizDTO, Quiz>(model);
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Quiz/AddQuiz", quiz);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public async Task<List<ListQuizDTO>> ListeQuiz()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync("/api/Quiz/ListQuiz");
            IEnumerable<ListQuizDTO>? listQuiz = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<ListQuizDTO>>();
            return listQuiz.ToList();
   
        }
        public async Task<Response> BindQuiz(QuizUserDTO quizUser)
        {
            QuizUser quiz = _mapper.Map<QuizUserDTO, QuizUser>(quizUser);
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Quiz/BindQuiz", quiz);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }

        public async Task<Response> DeleteQuiz(string id) 
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync("/api/Quiz/DeleteQuiz?id="+id,null);
            Response response = await httpResponseMessage.Content.ReadFromJsonAsync<Response>();
            return response;
        }
    }
}
