using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.Categorie_DTO;
using QuizApp.Entities.Conception_Entities.DTO.Quiz_DTO;

namespace Authentication.web.Pages
{
    public partial class CreateQuiz
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public string txtsnakError;
        CreationQuizDTO model = new CreationQuizDTO();
        List<ListCategorieDTO> _categories = new();

        protected override async Task OnInitializedAsync()
        {
            _categories = await categorieService.ListeCategorie();
        }

        private bool Validate()
        {
            if (model.titre == null)
            {
                txtsnakError = "<div>titre obligatoire</div>";
                SnackbarService.Add(txtsnakError);
            }
            if (model.description == null)
            {
                txtsnakError = "<div>description obligatoire</div>";
                SnackbarService.Add(txtsnakError);
            }
            if (model.categorieId == null)
            {
                txtsnakError = "<div>catégorie obligatoire</div>";
                SnackbarService.Add(txtsnakError);
            }
            if (model.nbr_questions == 0)
            {
                txtsnakError = "<div>nbr_questions obligatoire</div>";
                SnackbarService.Add(txtsnakError);
            }
            if (SnackbarService.ShownSnackbars.Count() == 0)
                return true;
            return false;

        }
        public async Task OnValidSubmit()
        {
            if (!Validate()) return;

            var Response = await quizService.CreateQuiz(model);
            if (Response.status)
            {
                SnackbarService.Add
                       ("Quiz enregistré avec succès", Severity.Success
                       );
                NavigationManager.NavigateTo("/QuizManagement");
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
