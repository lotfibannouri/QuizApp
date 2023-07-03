using Authentication.web.Services;
using BlazorMonaco.Editor;
using Microsoft.AspNetCore.Components;

namespace Authentication.web.Shared.Questions
{
    public partial class CodingQ
    {
        [Inject]
        private  IJdoodleService jdoodleService { get; set; }
        public string Language { get; set; }
        public string code { get; set; }
        public StandaloneCodeEditor MonacoRef;

        private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = "csharp",
                Value = "using System;\n" +
           " public class HelloWorld{ \n" +
           " public static void Main(string[] args)\n" +
            "{\n" +
            "};\n"
            };
            }

        public async Task OnShowOutPut()
        {
           var code = await MonacoRef.GetValue();
           var result =  await jdoodleService.GetOutput(code, Language, "4");
            Console.WriteLine(result);

        }

    }

}



