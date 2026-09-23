using Authentication.web.Services;
using Authentication.web.Shared;
using Authentication.web.Shared.Questions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.Categorie_DTO;
using QuizApp.Entities.Conception_Entities.DTO.Proposition_DTO;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Reponse_DTO;

namespace Authentication.web.Pages
{
    public partial class QuestionManagement

    {
        [Parameter]
        public string? QuestionId { get; set; }

        [Inject]
        public IQuestionService questionService { get; set; }

        [Inject]
        public NavigationManager _navigationManager { get; set; }

        public CreationQuestionDTO model = new CreationQuestionDTO();
        private enum WizardStep { ChooseType, CommonFields, CodingEditor }
        private WizardStep _step = WizardStep.ChooseType;
        private List<ListCategorieDTO> _categories = new();
        private bool _loading;

        private bool IsEditMode => !string.IsNullOrEmpty(QuestionId);

        protected override void OnInitialized()
        {
            // Exécuté avant le premier rendu : en modification on positionne l'étape tout de
            // suite et on affiche un indicateur de chargement, sinon Blazor rendrait d'abord
            // l'écran de choix du type le temps que le chargement asynchrone se termine.
            if (IsEditMode)
            {
                _step = WizardStep.CommonFields;
                _loading = true;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _categories = await categorieService.ListeCategorie();

            if (IsEditMode)
            {
                await LoadQuestionForEdit();
                _loading = false;
            }
        }

        private async Task LoadQuestionForEdit()
        {
            var question = await questionService.GetQuestionsById(QuestionId);
            if (question == null)
                return;

            model = new CreationQuestionDTO
            {
                questionText = question.questionText,
                description = question.description,
                type = question.type,
                note = question.note,
                categorieId = question.categorieId,
                propositions = new List<PropositionDTO>(),
                reponses = new List<CreationReponseDTO>()
            };

            // La correction d'une proposition n'est pas portée par la proposition elle-même :
            // elle vit dans la réponse dont le Body correspond au texte de la proposition.
            if (question.propositions != null)
            {
                foreach (var proposition in question.propositions)
                {
                    bool isAnswer = question.reponses != null &&
                        question.reponses.Any(r => r.Body == proposition.textProposition && r.IsAnswer);

                    model.propositions.Add(new PropositionDTO
                    {
                        _textPropositon = proposition.textProposition,
                        _chkProposition = isAnswer
                    });
                }
            }

            if (question.reponses != null)
            {
                foreach (var reponse in question.reponses)
                {
                    model.reponses.Add(new CreationReponseDTO
                    {
                        Body = reponse.Body,
                        IsRawAnswer = reponse.IsRawAnswer,
                        IsAnswer = reponse.IsAnswer,
                        Output = reponse.output,
                        Language = reponse.Language
                    });
                }
            }

            // En modification on saute le choix du type : on arrive directement sur les champs
            // communs, qui affichent déjà le composant correspondant au type de la question.
            _step = WizardStep.CommonFields;
        }

        private void SelectType(string type) => model.type = type;
        private void ConfirmType() => _step = WizardStep.CommonFields;
        private void EditType() => _step = WizardStep.ChooseType;

        private void GoToCodingEditor() => _step = WizardStep.CodingEditor;
        private void BackToCommonFields() => _step = WizardStep.CommonFields;

        private bool CanAdvanceToCoding =>
            !string.IsNullOrWhiteSpace(model.questionText) && !string.IsNullOrEmpty(model.categorieId);

        private void HandleSaved()
        {
            if (IsEditMode)
            {
                _navigationManager.NavigateTo("/QuestionBank");
                return;
            }

            model = new CreationQuestionDTO();
            _step = WizardStep.ChooseType;
        }

        private string CardStyle(string type) => model.type == type
            ? "cursor:pointer;border:2px solid var(--mud-palette-primary);"
            : "cursor:pointer;";
    }
}
