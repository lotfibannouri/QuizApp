using Authentication.web.Services;
using Authentication.web.utility;
using Microsoft.AspNetCore.Components;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using System.Timers;
namespace Authentication.web.Pages
{
    public partial class QuizTestVisuliser
    {
        [Parameter]
        public string QuizId { get; set; }

        [Inject]
        public IQuestionService _questionService { get; set; }
        [Inject]
        public IQuizService _quizService { get; set; }
        public List<ListQuestionDTO> _questions { get; set; }

        public List<IQuestionPersist> questionPersists { get; set; }

        private TimeSpan totalTime = TimeSpan.FromMinutes(0.1);
        private TimeSpan remainingTime;
        private System.Timers.Timer timer;
        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(QuizId))
            {  
                _questions = await _questionService.GetQuestionsByQuizId(QuizId);
                questionPersists = new List<IQuestionPersist>();
            }

            remainingTime = totalTime;

            timer = new System.Timers.Timer(1000); // 1 second interval
            timer.Elapsed += TimerElapsed;
            timer.Start();

        }
        public void getScore()
        {
            foreach(var question in questionPersists) 
            {
                question.save();
            }
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));

            if (remainingTime.TotalSeconds <= 0)
            {
                timer.Stop();
                TimerCompleted();
            }

            InvokeAsync(StateHasChanged);
        }

        private void TimerCompleted()
        {
            getScore();
        }
    }
}
