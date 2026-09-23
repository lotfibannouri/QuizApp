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

        [Parameter]
        public string? QuestionId { get; set; }

        [Parameter]
        public EventCallback OnSaved { get; set; }

        public List<PropositionDTO> propositions = new List<PropositionDTO>();

        private bool IsEditMode => !string.IsNullOrEmpty(QuestionId);

        protected override void OnInitialized()
        {
            // En modification, la liste locale doit être alimentée depuis la question chargée,
            // sinon le formulaire s'affiche vide malgré les données déjà présentes dans data.
            if (data?.propositions != null && data.propositions.Any())
            {
                propositions = data.propositions.ToList();
            }
        }

        private void OnDeleteProp(PropositionDTO prop)
        {
            propositions.Remove(prop);
        }

        private void OnAddProp()
        {
            propositions.Add(new PropositionDTO());

        }

        private bool CanSave =>
            !string.IsNullOrWhiteSpace(data?.questionText) &&
            !string.IsNullOrWhiteSpace(data?.categorieId) &&
            propositions.Count(p => !string.IsNullOrWhiteSpace(p._textPropositon)) >= 2;

        private async Task SaveQuestion()
        {

            data.propositions = propositions;
            List<CreationReponseDTO> rep = new List<CreationReponseDTO>();
            foreach(var item in propositions)
            {
                rep.Add(new CreationReponseDTO() {IsRawAnswer=true,Body=item._textPropositon,IsAnswer = item._chkProposition });
            }
            data.reponses = rep;

            var response = IsEditMode
                ? await Questionservice.UpdateQuestion(QuestionId, data)
                : await Questionservice.CreateQuestion(data);

            if (response != null && response.status)
            {
                await OnSaved.InvokeAsync();
            }

        }

    }


}
