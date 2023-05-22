using Authentication.web.Shared;
using Authentication.web.Shared.Questions;
using Microsoft.AspNetCore.Components;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;

namespace Authentication.web.Pages
{
    public partial class QuestionManagement
    {
        List<RenderFragment> _propositionsList = new List<RenderFragment>();
        public string selectedOption { get; set; }
        public string TextValue { get; set; }
        CreationQuestionDTO model = new CreationQuestionDTO();

        public void OnValidSubmit() {

           
        }

        public void OnAddProposition<TComponent>() where TComponent : IComponent
        {
            RenderFragment childComponent = builder =>
            {
                builder.OpenComponent(0, typeof(TComponent));
                builder.CloseComponent();
            };

            _propositionsList.Add(childComponent);
        }

        public void OnSelectedValueChanged() 
        {
            _propositionsList.Clear();
            TextValue = "";

        }




    }
}
