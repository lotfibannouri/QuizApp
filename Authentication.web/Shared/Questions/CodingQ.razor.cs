using Authentication.web.Services;
using BlazorMonaco;
using BlazorMonaco.Editor;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.Reponse_DTO;
using System.Reflection;

namespace Authentication.web.Shared.Questions
{
    public partial class CodingQ
    {
        [Parameter]
        public CreationQuestionDTO data { get; set; }
        [Parameter]
        public string? QuestionId { get; set; }
        [Parameter]
        public EventCallback OnSaved { get; set; }

        private bool IsEditMode => !string.IsNullOrEmpty(QuestionId);
        [Inject]
        private IJdoodleService jdoodleService { get; set; }
        [Inject]
        private IQuestionService Questionservice { get; set; }
        [Inject]
        private IDialogService dialogService { get; set; }
        public string _Language { get; set; }
        public string code { get; set; }
        public string output { get; set; }
        public StandaloneCodeEditor MonacoRef;
        public StandaloneEditorConstructionOptions options { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // En modification, le code et le langage de la question existante doivent être
            // repris : poser Value sur les options de construction suffit à pré-remplir Monaco.
            var reponse = data?.reponses?.FirstOrDefault();

            _Language = reponse?.Language ?? _Language;

            options = new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = reponse?.Language ?? "csharp",
                Value = reponse?.Body ?? string.Empty,
            };
        }

        private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
        {
            return options;
        }

        public async Task OnShowOutPut()
        {
            var code = await MonacoRef.GetValue();

            output = await jdoodleService.GetOutput(code, _Language, "4");
            StateHasChanged();
        }

        public async void OnChangeLanguage()
        {
            options.Language = _Language;
        }

        private async void SaveQuestion()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
            var parameters = new DialogParameters();
            IDialogReference dialogresult;
            if (_Language.IsNullOrEmpty())
            { parameters.Add("AlertMessage", "Vous devez choisir un Language!!!");
                dialogresult = await dialogService.ShowAsync<AlertBox>("Erreur", parameters, options);
                return;
            }
            parameters.Add("AlertMessage", "�te-vous s�re de bien vouloir sauvegarder cette question!!!");
            dialogresult = await dialogService.ShowAsync<AlertBox>("Validation", parameters, options);
            var result = await dialogresult.Result;
            if (result.Cancelled)
                return;
            var code = await MonacoRef.GetValue();
            if(code.IsNullOrEmpty())
            {
                parameters.Add("AlertMessage", "vous devez saisir un code");
                dialogresult = await dialogService.ShowAsync<AlertBox>("Erreur", parameters, options);
                return;
            }
            var resultServer = await jdoodleService.GetOutput(code, _Language, "4");
            data.reponses = new List<CreationReponseDTO>() { new CreationReponseDTO() { Body = code, IsRawAnswer = true, Output = resultServer, Language = _Language } };
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





