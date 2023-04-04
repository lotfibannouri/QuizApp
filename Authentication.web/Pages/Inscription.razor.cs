using Authentication.web.Model;
using Microsoft.AspNetCore.Components.Forms;

namespace Authentication.web.Pages
{
    public partial class Inscription
    {
        public SignUpModel model = new SignUpModel();
        public bool success;



        private async void OnValidSubmit()
        {
            success = true;
            StateHasChanged();
            var Response = await service.SignUpAsync(model);
            Console.WriteLine(Response);

        }
    }
}
