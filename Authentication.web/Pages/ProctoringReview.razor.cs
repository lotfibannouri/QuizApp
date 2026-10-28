using Authentication.web.Model;
using Authentication.web.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.Proctoring_DTO;

namespace Authentication.web.Pages
{
    public partial class ProctoringReview
    {
        private IEnumerable<ProctoringSessionDTO> Sessions = new List<ProctoringSessionDTO>();
        private string _searchString;

        private ProctoringSessionDTO? _selected;
        private List<ProctoringEventDTO> _snapshots = new();
        private List<ProctoringEventDTO> _incidents = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadSessions();
        }

        private async Task LoadSessions()
        {
            var sessions = await proctoringService.ListSessions();

            // L'API des quiz ne connaît que des identifiants : les noms viennent de l'API
            // d'authentification, résolus ici en une seule requête pour tous les étudiants.
            var users = await adminService.GetUsers();
            // Comparaison insensible à la casse : les deux API sérialisent leurs GUID
            // indépendamment, un écart de casse afficherait des identifiants bruts.
            var usersById = users.ToDictionary(u => u.id, u => u, StringComparer.OrdinalIgnoreCase);

            foreach (var session in sessions)
            {
                session.UserName = usersById.TryGetValue(session.UserId, out User user)
                    ? user.userName
                    : session.UserId;
            }

            Sessions = sessions;
        }

        private Func<ProctoringSessionDTO, bool> _quickFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;

            if (!string.IsNullOrWhiteSpace(x.UserName) && x.UserName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrWhiteSpace(x.QuizTitre) && x.QuizTitre.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.StartedAt.ToString().Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        private async Task OpenSession(DataGridRowClickEventArgs<ProctoringSessionDTO> args)
        {
            _selected = args.Item;

            var events = await proctoringService.GetSessionEvents(_selected.SessionId);
            _snapshots = events.Where(e => e.HasImage).ToList();
            _incidents = events.Where(e => !e.HasImage).ToList();
        }

        private void CloseSession()
        {
            _selected = null;
            _snapshots = new();
            _incidents = new();
        }

        private async Task PurgeSession()
        {
            if (_selected == null)
                return;

            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, FullWidth = true };
            var parameters = new DialogParameters();
            parameters.Add("AlertMessage", "Supprimer définitivement toutes les captures de cette session ?");
            var dialogresult = await dialogService.ShowAsync<AlertBox>("Validation", parameters, options);
            var result = await dialogresult.Result;
            if (result.Cancelled)
                return;

            Response response = await proctoringService.PurgeSession(_selected.SessionId);
            SnackbarService.Add($"<div>{response.content}</div>", response.status ? Severity.Success : Severity.Warning);

            if (response.status)
            {
                CloseSession();
                await LoadSessions();
            }
        }
    }
}
