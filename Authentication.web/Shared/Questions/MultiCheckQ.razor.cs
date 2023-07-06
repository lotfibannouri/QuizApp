using Microsoft.AspNetCore.Components;
using QuizApp.Entities.Conception_Entities.DTO.Proposition_DTO;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Reponse_DTO;

namespace Authentication.web.Shared.Questions
{
    public partial class MultiCheckQ
    {

        [Parameter]
        public CreationQuestionDTO data { get; set; }
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

       
        }

    }


}
