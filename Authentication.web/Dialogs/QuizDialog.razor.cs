using Authentication.web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace Authentication.web.Dialogs
{
    public partial class QuizDialog
    {
        public string txtsnakError = "<div>Problème Serveur</div>";
        CreationQuizDTO model = new CreationQuizDTO();
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        public async Task OnValidSubmit()
        {
            var Response = await quizService.CreateQuiz(model);
            if(Response.status)
            {
                MudDialog.Close(DialogResult.Ok(true));
            }
            else
            {
                SnackbarService.Add
                       (txtsnakError, Severity.Warning
                       );
            }
        }

    }
}
