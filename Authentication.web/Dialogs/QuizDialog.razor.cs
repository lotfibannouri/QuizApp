using Authentication.web.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace Authentication.web.Dialogs
{
    public partial class QuizDialog
    {
        public string txtsnakError ;
        CreationQuizDTO model = new CreationQuizDTO();
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        private bool Validate()
        {
            if (model.titre == null) 
            {
                 txtsnakError = "<div>titre obligatoire</div>";
                SnackbarService.Add(txtsnakError);
            }
            if(model.description == null)
            {
                txtsnakError = "<div>descrition obligatoire</div>";
                SnackbarService.Add(txtsnakError);
            }
            if (model.niv_deficulte==0)
            {
                txtsnakError = "<div>niveau de difficulté obligatoire</div>";
                SnackbarService.Add(txtsnakError);
            }
            if (model.nbr_questions==0)
            {
                txtsnakError = "<div>nbr_questions obligatoire</div>";
                SnackbarService.Add(txtsnakError);
            }
            if(SnackbarService.ShownSnackbars.Count()==0)
                 return true;
            return false;

        }
        public async Task OnValidSubmit()
        {
            if (!Validate()) return;

            var Response = await quizService.CreateQuiz(model);
            if(Response.status)
            {
                MudDialog.Close(DialogResult.Ok(true));
                SnackbarService.Add
                       ("Quiz enregistré avec succès", Severity.Success
                       );
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
