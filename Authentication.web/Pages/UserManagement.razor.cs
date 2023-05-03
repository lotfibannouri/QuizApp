using Authentication.web.Model;
using Authentication.web.Shared;
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
            _events.Insert(0, $"Event = SelectedItemsChanged, data = {System.Text.Json.JsonSerializer.Serialize(items)}");
        }



        private async Task AddUser()
        {



            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true  , FullWidth = true};
            var parameters = new DialogParameters();
            parameters.Add("ContentText", "Do you want to confirm?");
            parameters.Add("ButtonText", "Yes");

            var dialogresult = await dialogService.ShowAsync<UserDialog>("Création des Utilisateurs",options);
            var result = await dialogresult.Result;
            Users = await adminService.GetUsers();

            //if (!result.Cancelled && bool.TryParse(result.Data.ToString(), out bool resultbool)) IncrementCount();

            //var dialogResult = dialogService.Show<UserDialog>("AddUserDialog", options);

        }
    }
}
