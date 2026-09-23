using Authentication.web.Services;
using Authentication.web.utility;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using System.Security.Cryptography.X509Certificates;

namespace Authentication.web.Shared.Test
{
    public partial class TestMultichk : ComponentBase,IQuestionPersist
    {
        [Parameter]
        public ListQuestionDTO Question { get; set; }

        [Inject]
        public IQuestionService _questionService { get; set; }

        public List<checkeditem> chkdItems { get; set; } = new List<checkeditem>();

        public async Task<double> save()
        {
            foreach (var item in chkdItems)
            {
                var mapped = Question.reponses.FirstOrDefault(x => x.Body == item.text);
                item.color = mapped.IsAnswer == item.ischecked ? Color.Success : Color.Error;
            }
            this.StateHasChanged();

            var request = new ScoreRequestDTO
            {
                QuestionId = Question.Id,
                Answers = chkdItems.Select(item => new AnswerSubmissionDTO { Body = item.text, IsChecked = item.ischecked }).ToList()
            };

            return await _questionService.CalculateMultiChoiceScore(request);
        }

        protected override async Task OnInitializedAsync()
        {
            foreach(var item in Question.reponses)
            {
                chkdItems.Add(new checkeditem { text=item.Body , ischecked = false, chkboxref = new MudCheckBox<bool>() });
            }         
        }
    }
    public class checkeditem
    {
        public string text;
        public bool ischecked;
        public MudCheckBox<bool> chkboxref;
        public Color color = Color.Primary;
    }
}
