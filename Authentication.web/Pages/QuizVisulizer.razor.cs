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
        public ListQuizDTO Quiz { get; set; }
        List<ListQuestionDTO> listQuestionDTOs { get; set; }    
        protected override async Task OnInitializedAsync()
        {
           if(!string.IsNullOrEmpty(QuizId))
            {
                Quiz = await _quizService.GetQuizById(QuizId);
                listQuestionDTOs = await _questionService.GetQuestionsByQuizId(QuizId);

            }
        }
    }
}
