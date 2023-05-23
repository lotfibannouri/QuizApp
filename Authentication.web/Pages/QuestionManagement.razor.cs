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
            var newval = _propositionsList.Count();
            RenderFragment childComponent = builder =>
            {
                builder.OpenComponent(0, typeof(TComponent));
                builder.AddAttribute(1, "index", newval);
                builder.AddAttribute(2, "OnDeleteEventClicked", EventCallback.Factory.Create<int>(this,OnDeleteItem));
                builder.CloseComponent();
            };
           
            _propositionsList.Add(childComponent);
        }

        public void OnSelectedValueChanged() 
        {
            _propositionsList.Clear();
            TextValue = "";

        }

        public void OnDeleteItem(int index)
         {
            _propositionsList.RemoveAt(index);
         }


    }
}
