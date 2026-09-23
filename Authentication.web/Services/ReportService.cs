using AutoMapper;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.ReportDTO;
using System.Net.Http;
using System.Net.Http.Json;

namespace Authentication.web.Services
{
    public class ReportService : IReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public ReportService(HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public async Task<byte[]> GetReport(QuizReportRequestDTO ReportRequest)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Quiz/GetQuizReport", ReportRequest);
            var response = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            return response;
        }

        public async Task<QuizScoreSummaryDTO> GetScoreSummary(QuizReportRequestDTO ReportRequest)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync("/api/Quiz/GetQuizScoreSummary", ReportRequest);
            return await httpResponseMessage.Content.ReadFromJsonAsync<QuizScoreSummaryDTO>();
        }
    }
}
