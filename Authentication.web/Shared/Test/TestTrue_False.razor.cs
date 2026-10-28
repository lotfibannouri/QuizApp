using Authentication.web.Services;
using Authentication.web.utility;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;

namespace Authentication.web.Shared.Test
{
    public partial class TestTrue_False : ComponentBase, IQuestionPersist
    {

        [Parameter]
        public ListQuestionDTO Question { get; set; }

        [Inject]
        public IQuestionService _questionService { get; set; }

        public List<TrueFalseItem> chkdItems { get; set; } = new List<TrueFalseItem>();

        public async Task<double> save()
        {
            foreach (var item in chkdItems)
            {
                // Recherche par texte : chercher par IsAnswer comparait la réponse à elle-même.
                var mapped = Question.reponses.FirstOrDefault(x => x.Body == item.text);
                item.color = mapped.IsAnswer == item.ischecked ? Color.Success : Color.Error;
            }
            this.StateHasChanged();

            var request = new ScoreRequestDTO
            {
                QuestionId = Question.Id,
                Answers = BuildAnswers()
            };

            return await _questionService.CalculateTrueFalseScore(request);
        }

        /// <summary>
        /// Seules les affirmations répondues sont envoyées : une affirmation absente est
        /// considérée sans réponse par l'API et ne rapporte aucun point.
        /// </summary>
        public List<AnswerSubmissionDTO> BuildAnswers()
            => chkdItems
                .Where(item => item.ischecked.HasValue)
                .Select(item => new AnswerSubmissionDTO { Body = item.text, IsChecked = item.ischecked.Value })
                .ToList();

        protected override async Task OnInitializedAsync()
        {
            foreach (var item in Question.reponses)
            {
                chkdItems.Add(new TrueFalseItem { text = item.Body });
            }
        }
    }

    public class TrueFalseItem
    {
        public string text;
        /// <summary>true = Vrai, false = Faux, null = pas encore répondu.</summary>
        public bool? ischecked = null;
        public Color color = Color.Primary;
    }

}


