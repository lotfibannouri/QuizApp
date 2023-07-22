using Authentication.web.utility;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using System.Security.Cryptography.X509Certificates;

namespace Authentication.web.Shared.Test
{
    public partial class TestMultichk : IQuestionPersist
    {
        [Parameter]
        public ListQuestionDTO _question { get; set; }
        public List<checkeditem> chkdItems { get; set; } = new List<checkeditem>();
        public long save()
        {
            foreach(var item in chkdItems)
            {
                var mapped = _question.reponses.FirstOrDefault(x => x.Body == item.text);
                if (mapped.IsAnswer != item.ischecked)
                    item.chkboxref.Color = Color.Error;
            }
            return 0;
          
        }

        protected override async Task OnInitializedAsync()
        {
            foreach(var item in _question.reponses)
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
    }
}
