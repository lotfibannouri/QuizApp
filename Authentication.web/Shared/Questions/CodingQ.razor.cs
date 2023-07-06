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
        [Inject]
        private IJdoodleService jdoodleService { get; set; }
        [Inject]
        private IQuestionService Questionservice { get; set; }
        [Inject]
        private IDialogService dialogService { get; set; }
        public string _Language { get; set; }
        public string code { get; set; }
        public StandaloneCodeEditor MonacoRef;
        public StandaloneEditorConstructionOptions options { get; set; }

        protected override async Task OnInitializedAsync()
        {
            options = new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = "csharp",

            };
        }

        private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
        {
            return options;
        }

        public async Task OnShowOutPut()
        {
            var code = await MonacoRef.GetValue();

            var result = await jdoodleService.GetOutput(code, _Language, "4");
            Console.WriteLine(result);
            var model = await MonacoRef.GetModel();
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
            parameters.Add("AlertMessage", "ête-vous sûre de bien vouloir sauvegarder cette question!!!");
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
            data.reponses = new List<CreationReponseDTO>() { new CreationReponseDTO() { Body = code, IsRawAnswer = true, Output = resultServer } };
            var response = await Questionservice.CreateQuestion(data);
        }

    }

}





