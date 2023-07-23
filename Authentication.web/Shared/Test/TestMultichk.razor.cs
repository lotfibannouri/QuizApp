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
        public List<checkeditem> chkdItems { get; set; } = new List<checkeditem>();
        public double save()
        {
            double score=0;
            foreach(var item in chkdItems)
            {
                
                var mapped = Question.reponses.FirstOrDefault(x => x.Body == item.text);
                if (mapped.IsAnswer != item.ischecked)
                {
                    item.color = Color.Error;
                    score-=0.25;
                }
                else
                {
                    item.color = Color.Success;
                    score += 0.25;
                }
                    
            }
            this.StateHasChanged();
            return score;
          
        }

        protected override async Task OnInitializedAsync()
        {
            foreach(var item in Question.reponses)
            {
                chkdItems.Add(new checkeditem { text=item.Body , ischecked = false, chkboxref = new MudCheckBox<bool>()});
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
