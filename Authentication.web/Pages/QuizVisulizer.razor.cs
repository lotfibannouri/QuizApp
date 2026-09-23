using Authentication.web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace Authentication.web.Pages
{
    public partial class QuizVisulizer : ComponentBase
    {
        [Parameter]
        public string QuizId { get; set; }
        [Inject]
        public IQuestionService _questionService { get; set; }
        [Inject]
        public IQuizService _quizService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }

        public ListQuizDTO _quiz { get; set; }
        public List<ListQuestionDTO> _questions { get; set; }

        public List<string> listLogos = new() { Icons.Material.Filled.Quiz, Icons.Material.Filled.Code, Icons.Material.Filled.School };

        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(QuizId))
            {
                _quiz = await _quizService.GetQuizById(QuizId);
                _questions = await _questionService.GetQuestionsByQuizId(QuizId);
            }
        }

        private string QuizIcon => _quiz != null && _quiz.icon >= 0 && _quiz.icon < listLogos.Count
            ? listLogos[_quiz.icon]
            : Icons.Material.Filled.Quiz;

        private string QuestionTypeIcon(string type) => type switch
        {
            "Choix Multiple" => Icons.Material.Filled.CheckBox,
            "Vrai/Faux" => Icons.Material.Filled.RuleFolder,
            "Code" => Icons.Material.Filled.Terminal,
            _ => Icons.Material.Filled.HelpOutline
        };

        private void GoBack() => _navigationManager.NavigateTo("/QuizManagement");
    }
}
