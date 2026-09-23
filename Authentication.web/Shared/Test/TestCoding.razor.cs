using Authentication.web.Services;
using Authentication.web.utility;
using BlazorMonaco;
using BlazorMonaco.Editor;
using Microsoft.AspNetCore.Components;
using QuizApp.Entities.Conception_Entities.DTO.QuestionDTO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication.web.Shared.Test
{
    public partial class TestCoding : ComponentBase, IQuestionPersist
    {
        private const string StartMarker = "// DEBUT";
        private const string EndMarker = "// FIN";

        [Parameter]
        public ListQuestionDTO Question { get; set; }

        [Inject]
        public IJdoodleService jdoodleService { get; set; }

        public StandaloneCodeEditor PrefixEditorRef;
        public StandaloneCodeEditor MiddleEditorRef;
        public StandaloneCodeEditor SuffixEditorRef;

        public StandaloneEditorConstructionOptions prefixOptions;
        public StandaloneEditorConstructionOptions middleOptions;
        public StandaloneEditorConstructionOptions suffixOptions;

        public string output { get; set; }
        public bool HasTemplate { get; private set; }

        private const int LineHeightPx = 19;

        private string prefix = string.Empty;
        private string suffix = string.Empty;

        public int PrefixHeightPx { get; private set; } = LineHeightPx + 8;
        public int SuffixHeightPx { get; private set; } = LineHeightPx + 8;

        private string Language => Question.reponses?.FirstOrDefault()?.Language ?? "csharp";

        protected override void OnInitialized()
        {
            var body = Question.reponses?.FirstOrDefault()?.Body ?? string.Empty;
            var lines = body.Replace("\r\n", "\n").Split('\n');

            // Recherche tolérante : le marqueur peut être seul sur sa ligne (ex. "// FIN")
            // ou collé à du code sur la même ligne (ex. "{// DEBUT"), donc on ne fait pas
            // une égalité stricte de la ligne entière mais on cherche le marqueur dedans.
            var startIndex = Array.FindIndex(lines, l => l.Contains(StartMarker, StringComparison.OrdinalIgnoreCase));
            var endIndex = Array.FindIndex(lines, l => l.Contains(EndMarker, StringComparison.OrdinalIgnoreCase));

            HasTemplate = startIndex >= 0 && endIndex > startIndex;

            if (HasTemplate)
            {
                prefix = string.Join("\n", lines.Take(startIndex + 1));
                suffix = string.Join("\n", lines.Skip(endIndex));

                PrefixHeightPx = Math.Max(1, startIndex + 1) * LineHeightPx + 8;
                SuffixHeightPx = Math.Max(1, lines.Length - endIndex) * LineHeightPx + 8;
            }

            var readonlyScrollbar = new EditorScrollbarOptions
            {
                VerticalScrollbarSize = 0,
                HorizontalScrollbarSize = 0
            };

            prefixOptions = new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = Language,
                Value = prefix,
                ReadOnly = true,
                FontSize = 14,
                LineHeight = LineHeightPx,
                ScrollBeyondLastLine = false,
                Minimap = new EditorMinimapOptions { Enabled = false },
                Scrollbar = readonlyScrollbar
            };
            middleOptions = new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = Language,
                Value = string.Empty,
                FontSize = 14,
                LineHeight = LineHeightPx,
                ScrollBeyondLastLine = false
            };
            suffixOptions = new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = Language,
                Value = suffix,
                ReadOnly = true,
                FontSize = 14,
                LineHeight = LineHeightPx,
                ScrollBeyondLastLine = false,
                Minimap = new EditorMinimapOptions { Enabled = false },
                Scrollbar = readonlyScrollbar
            };
        }

        private StandaloneEditorConstructionOptions PrefixConstructionOptions(StandaloneCodeEditor editor) => prefixOptions;
        private StandaloneEditorConstructionOptions MiddleConstructionOptions(StandaloneCodeEditor editor) => middleOptions;
        private StandaloneEditorConstructionOptions SuffixConstructionOptions(StandaloneCodeEditor editor) => suffixOptions;

        private async Task<string> BuildFullCodeAsync()
        {
            var middleCode = await MiddleEditorRef.GetValue();
            if (!HasTemplate)
                return middleCode;

            return prefix + "\n" + middleCode + "\n" + suffix;
        }

        public async Task<(string Code, string Output)> GetSubmission()
        {
            var code = await BuildFullCodeAsync();
            var output = await jdoodleService.GetOutput(code, Language, "4");
            return (code, output ?? string.Empty);
        }

        public async Task OnRun()
        {
            var code = await BuildFullCodeAsync();
            output = await jdoodleService.GetOutput(code, Language, "4");
            StateHasChanged();
        }

        public async Task<double> save()
        {
            var code = await BuildFullCodeAsync();
            var result = await jdoodleService.GetOutput(code, Language, "4");
            var expected = Question.reponses?.FirstOrDefault()?.output;
            return result?.Trim() == expected?.Trim() ? 1 : 0;
        }
    }
}
