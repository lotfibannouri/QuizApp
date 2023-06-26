using BlazorMonaco.Editor;

namespace Authentication.web.Shared.Questions
{
    public partial class CodingQ
    {
        public string Language { get; set; }

        private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = "javascript",
                Value = "function xyz() {\n" +
                        "   console.log(\"Hello world!\");\n" +
                        "}"
            };
        }
    }

}


