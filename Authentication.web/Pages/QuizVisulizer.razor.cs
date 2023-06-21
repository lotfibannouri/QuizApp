using Authentication.web.Services;
using Microsoft.AspNetCore.Components;
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
        public ListQuizDTO _quiz { get; set; }
        public List<ListQuestionDTO> _questions { get; set; }
        public bool QuestionView { get; set; }
        public bool QuizView { get; set; }
        public bool ChoiseView { get; set; }
        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(QuizId))
            {
                _quiz = await _quizService.GetQuizById(QuizId);
                _questions = await _questionService.GetQuestionsByQuizId(QuizId);
            }
            QuizView = false;
            QuestionView = true;
            ChoiseView = true;         
        }


        private void ActiveView(int value)
        {
            switch (value)
            {
                case 0:
                    QuizView = false;
                    QuestionView= true;
                    ChoiseView= true;
                    break;
                case 1:
                    QuizView = true;
                    QuestionView = false;
                    ChoiseView = true;
                    break;    
                case 2:
                    QuizView = true;
                    QuestionView = true;
                    ChoiseView = false;
                    break;   
            }
        }
    }
}
