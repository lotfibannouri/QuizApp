using Authentication.web.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using QuizApp.Entities.Conception_Entities.DTO.ResultatDTO;
using System.Text.Json;

namespace Authentication.web.Pages
{
    public partial class Resultats
    {
        private List<CandidatResultats> _candidats = new();
        private string _searchString;
        private bool _loading = true;

        private ResultatDetailDTO? _detail;
        private string? _detailCandidat;

        protected override async Task OnInitializedAsync()
        {
            var resultats = await resultatService.ListAll();

            // L'API des quiz ne connaît que des identifiants : noms et emails viennent de l'API
            // d'authentification, en une seule requête (même approche que ProctoringReview).
            var users = await adminService.GetUsers();
            var usersById = users.ToDictionary(u => u.id, u => u, StringComparer.OrdinalIgnoreCase);

            _candidats = resultats
                .GroupBy(r => r.UserId, StringComparer.OrdinalIgnoreCase)
                .Select(g =>
                {
                    usersById.TryGetValue(g.Key, out User user);
                    return new CandidatResultats
                    {
                        UserId = g.Key,
                        Nom = user != null ? $"{user.firstName} {user.lastName}" : g.Key,
                        Email = user?.email,
                        Passages = g.OrderByDescending(r => r.SubmittedAt).ToList()
                    };
                })
                .OrderBy(c => c.Nom)
                .ToList();

            _loading = false;
        }

        // Filtre sur le nom, l'email du candidat ou le titre d'un des quiz passés.
        private IEnumerable<CandidatResultats> FilteredCandidats => string.IsNullOrWhiteSpace(_searchString)
            ? _candidats
            : _candidats.Where(c =>
                c.Nom.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ||
                (c.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
                c.Passages.Any(p => p.QuizTitre?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) ?? false));

        private async Task OpenDetail(CandidatResultats candidat, ResultatDTO passage)
        {
            _detail = await resultatService.GetDetail(passage.Id);
            if (_detail == null)
            {
                SnackbarService.Add("Détail du résultat introuvable", Severity.Warning);
                return;
            }

            _detailCandidat = candidat.Nom;
        }

        private void CloseDetail()
        {
            _detail = null;
            _detailCandidat = null;
        }

        private static Color MentionColor(string mention) => mention switch
        {
            "Excellent" => Color.Warning,
            "Réussi" => Color.Success,
            _ => Color.Error
        };

        private static string FormatDuree(int secondes)
            => secondes >= 60 ? $"{secondes / 60} min {secondes % 60:00} s" : $"{secondes} s";

        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Réponses QCM / Vrai-Faux enregistrées en JSON (liste de Body + IsChecked).
        /// Une valeur illisible donne une liste vide plutôt qu'une erreur d'affichage.
        /// </summary>
        private static List<AnswerSubmissionDTO> ParseAnswers(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new();

            try
            {
                return JsonSerializer.Deserialize<List<AnswerSubmissionDTO>>(json, JsonOptions) ?? new();
            }
            catch (JsonException)
            {
                return new();
            }
        }

        private class CandidatResultats
        {
            public string UserId { get; set; }
            public string Nom { get; set; }
            public string? Email { get; set; }
            public List<ResultatDTO> Passages { get; set; } = new();

            public double MoyennePourcentage => Passages.Any() ? Passages.Average(p => p.Pourcentage) : 0;
            public double MeilleurPourcentage => Passages.Any() ? Passages.Max(p => p.Pourcentage) : 0;
            public int NbReussis => Passages.Count(p => p.IsReussi);
        }
    }
}
