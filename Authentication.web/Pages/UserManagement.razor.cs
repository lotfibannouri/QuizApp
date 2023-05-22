using Authentication.web.Model;
using Authentication.web.Services;
using Authentication.web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using System;
using System.Runtime.CompilerServices;

namespace Authentication.web.Pages
{

  
    public partial class UserManagement
    {
        private IEnumerable<User> Users = new List<User>();
        private int RowNumber = 0;
        private string _searchString;
        private bool _sortNameByLength;
        private List<string> _events = new();
        private HashSet<User> UsersSelected = new();
        private bool IsEditPage = false;
        public string txtsnakSuccess = "<div>suppression réussie</div>";
        public string txtsnakError = "<div>Problème de suppression</div>";
        [Inject]
        public IAdministrationService _administrationService { get; set; }

        // custom sort by name length

        private Func<User, object> _sortBy => x =>
        {
            if (_sortNameByLength)
                return x.lastName.Length;
            else
                return x.lastName;
        };
        // quick filter - filter gobally across multiple columns with the same input
        private Func<User, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (!string.IsNullOrWhiteSpace(x.lastName) && x.lastName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrWhiteSpace(x.firstName) && x.firstName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrWhiteSpace(x.rowNumber.ToString()) && x.rowNumber.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrWhiteSpace(x.adresse.ToString()) && x.email.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (!string.IsNullOrWhiteSpace(x.email.ToString()) && x.email.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            //if ($"{x.rowNumber} {x.adresse} {x.email}".Contains(_searchString))
            //    return true;

            return false;
        };

        protected override async Task OnInitializedAsync()
        {

            Users = await adminService.GetUsers();
        }

        // events
        void RowClicked(DataGridRowClickEventArgs<User> args)
        {
            _events.Insert(0, $"Event = RowClick, Index = {args.RowIndex}, Data = {System.Text.Json.JsonSerializer.Serialize(args.Item)}");
        }

        void SelectedItemsChanged(HashSet<User> items)
        {
            //UsersSelected = items.ToList();
            //_events.Insert(0, $"Event = SelectedItemsChanged, data = {System.Text.Json.JsonSerializer.Serialize(items)}");
        }



        private async Task AddUser()
        {

            IsEditPage = false;
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true  , FullWidth = true};
            var parameters = new DialogParameters();
            parameters.Add("dialogTitle", "Ajouter utilisateur");
            parameters.Add("IsEditPage", IsEditPage);
            var dialogresult = await dialogService.ShowAsync<UserDialog>("Création des Utilisateurs",parameters,options);
            var result = await dialogresult.Result;
            Users = await adminService.GetUsers();

            //if (!result.Cancelled && bool.TryParse(result.Data.ToString(), out bool resultbool)) IncrementCount();

            //var dialogResult = dialogService.Show<UserDialog>("AddUserDialog", options);

        }

        private async Task DeleteUser()
        {
            foreach (var item in UsersSelected)
            {
                Response response =  await _administrationService.DeleteUserAsync(item.id);
                if(response.status) 
                {
                    SnackbarService.Add
                        (txtsnakSuccess, Severity.Success
                        );

                }
                else
                {
                    SnackbarService.Add
                        (txtsnakError, Severity.Warning
                        );
                }
            }

            Users = await adminService.GetUsers();

        }   

        private async Task EditUser()
        {
            var optionsAlertBox = new DialogOptions { CloseOnEscapeKey = true };
            var parametersAlertBox = new DialogParameters();

            if (UsersSelected.Count() > 1)
            { 
              parametersAlertBox.Add("AlertMessage", "Fallai choisir un seul utilisateur!");
              await dialogService.ShowAsync<AlertBox>("Alert", parametersAlertBox, optionsAlertBox);
            }
            else if (UsersSelected.Count == 0 )
            {
                parametersAlertBox.Add("AlertMessage", "Fallait choisir un utilisateur!");
                await dialogService.ShowAsync<AlertBox>("Alert", parametersAlertBox, optionsAlertBox);
            }
            else { 
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
            IsEditPage = true;
            var parameters = new DialogParameters();
            parameters.Add("UserSelected", UsersSelected.ToList());
            parameters.Add("dialogTitle", "Modifier utilisateur");
            parameters.Add("IsEditPage", IsEditPage);
            var dialogresult = await dialogService.ShowAsync<UserDialog>("Modification des utilisateurs", parameters, options);
            }
            Users = await adminService.GetUsers();
        }

        private async Task AssignRoleToUser()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
            var parameters = new DialogParameters();
            IDialogReference? dialogresult;
            if (UsersSelected.Count() == 0)
            {
                parameters.Add("AlertMessage", "Fallait choisir un utilisateur!");
                dialogresult =  await dialogService.ShowAsync<AlertBox>("Alert", parameters, options);
            }
            else if (UsersSelected.Count() > 1)
                {
                parameters.Add("AlertMessage", "Fallait choisir un seul utilisateur!");
                dialogresult =await dialogService.ShowAsync<AlertBox>("Alert", parameters, options);
                }
            else { 
            parameters.Add("UserSelected", UsersSelected.ToList());
             dialogresult = await dialogService.ShowAsync<AssignUserRole>("Attribuer rôles", parameters, options);
            }
            var result = await dialogresult.Result;
            UsersSelected= new HashSet<User>();
            Users = await adminService.GetUsers();
            StateHasChanged();
        }
    }
}
