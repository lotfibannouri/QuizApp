using Microsoft.AspNetCore.Components;
using QuizApp.Entities.Conception_Entities.DTO.Proposition_DTO;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Reponse_DTO;

namespace Authentication.web.Shared.Questions
{
    public partial class MultiCheckQ
    {
<<<<<<< Updated upstream
        public List<int> QuestionsList = new List<int>() {1,2,3,4};
        public bool checkedItem { get; set; } = false;
        public string TextValue { get; set; } = "";
       
        [Parameter]
        public int index { get; set; }
        
        [Parameter]
        public EventCallback<int> OnClick { get; set; }

        private async Task InvokDeleteItem() 
        {
            OnClick.InvokeAsync();
=======

        [Parameter]
        public CreationQ_PropDTO data { get; set; }
        public List<PropositionDTO> propositions = new List<PropositionDTO>();
        public bool chkprop { get; set; }
        public bool responsePage { get; set; }
       
        private void OnDeleteProp(PropositionDTO prop)
        {
            propositions.Remove(prop);
        }

        private void OnAddProp()
        {
            propositions.Add(new PropositionDTO());

        }

        private void RenderResponse()
        {
            responsePage = true;
        }

        private void OnPrevious()
        {
            responsePage = false;
        }
        private void SaveQuestion() 
        {

            data.propositions = propositions;
            List<CreationReponseDTO> rep = new List<CreationReponseDTO>();
            foreach(var item in propositions)
            {if (item._chkProposition)
                    rep.Add(new CreationReponseDTO() {IsRawAnswer=true,Body=item._textPropositon});
            }
            data.reponses = rep;
            var response = Questionservice.CreateQuestion(data);
>>>>>>> Stashed changes
        }
        //testing
    }


}
