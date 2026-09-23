using Authentication.web.Model;
using Authentication.web.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.Categorie_DTO;

namespace Authentication.web.Pages
{
    public partial class CategorieManagement
    {
        private IEnumerable<ListCategorieDTO> Categories = new List<ListCategorieDTO>();
        private string _searchString;
        private HashSet<ListCategorieDTO> CategoriesSelected = new();
        public string txtsnakSuccess = "<div>suppression réussie</div>";
        public string txtsnakError = "<div>Problème de suppression</div>";

        protected override async Task OnInitializedAsync()
        {
            Categories = await categorieService.ListeCategorie();
        }

        private Func<ListCategorieDTO, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (!string.IsNullOrWhiteSpace(x.titre) && x.titre.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrWhiteSpace(x.description) && x.description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        void SelectedItemsChanged(HashSet<ListCategorieDTO> items)
        {
            CategoriesSelected = items;
        }

        private async Task AddCategorie()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
            var parameters = new DialogParameters();
            parameters.Add("dialogTitle", "Ajouter catégorie");
            parameters.Add("IsEditPage", false);
            var dialogresult = await dialogService.ShowAsync<CategorieDialog>("Création de catégorie", parameters, options);
            await dialogresult.Result;
            Categories = await categorieService.ListeCategorie();
            CategoriesSelected.Clear();
        }

        private async Task EditCategorie()
        {
            var optionsAlertBox = new DialogOptions { CloseOnEscapeKey = true };
            var parametersAlertBox = new DialogParameters();

            if (CategoriesSelected.Count > 1)
            {
                parametersAlertBox.Add("AlertMessage", "Il faut choisir une seule catégorie!");
                await dialogService.ShowAsync<AlertBox>("Alert", parametersAlertBox, optionsAlertBox);
            }
            else if (CategoriesSelected.Count == 0)
            {
                parametersAlertBox.Add("AlertMessage", "Il faut choisir une catégorie!");
                await dialogService.ShowAsync<AlertBox>("Alert", parametersAlertBox, optionsAlertBox);
            }
            else
            {
                var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
                var parameters = new DialogParameters();
                parameters.Add("CategorieSelected", CategoriesSelected.ToList());
                parameters.Add("dialogTitle", "Modifier catégorie");
                parameters.Add("IsEditPage", true);
                var dialogresult = await dialogService.ShowAsync<CategorieDialog>("Modification de catégorie", parameters, options);
                await dialogresult.Result;
            }
            Categories = await categorieService.ListeCategorie();
            CategoriesSelected.Clear();
        }

        private async Task DeleteCategorie()
        {
            if (CategoriesSelected.Count == 0)
            {
                var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
                var parameters = new DialogParameters();
                parameters.Add("AlertMessage", "Il faut choisir une catégorie!");
                await dialogService.ShowAsync<AlertBox>("Alert", parameters, options);
                CategoriesSelected.Clear();
                return;
            }
            foreach (var item in CategoriesSelected)
            {
                Response response = await categorieService.DeleteCategorie(item.Id);
                if (response.status)
                {
                    SnackbarService.Add(txtsnakSuccess, Severity.Success);
                }
                else
                {
                    SnackbarService.Add(txtsnakError, Severity.Warning);
                }
            }

            Categories = await categorieService.ListeCategorie();
            CategoriesSelected.Clear();
        }

        private async Task DetailedCategorie()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
            var parameters = new DialogParameters();

            if (CategoriesSelected.Count > 1)
            {
                parameters.Add("AlertMessage", "Il faut choisir une seule catégorie!");
                await dialogService.ShowAsync<AlertBox>("Alert", parameters, options);
                CategoriesSelected.Clear();
                return;
            }
            else if (CategoriesSelected.Count == 0)
            {
                parameters.Add("AlertMessage", "Il faut choisir une catégorie!");
                await dialogService.ShowAsync<AlertBox>("Alert", parameters, options);
                CategoriesSelected.Clear();
                return;
            }

            parameters.Add("AlertMessage", CategoriesSelected.First().description);
            parameters.Add("ShowActions", false);
            await dialogService.ShowAsync<AlertBox>("Description de la catégorie", parameters, options);
            CategoriesSelected.Clear();
        }
    }
}
