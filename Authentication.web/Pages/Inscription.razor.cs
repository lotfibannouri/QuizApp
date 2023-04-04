using Authentication.web.Model;
using Microsoft.AspNetCore.Components.Forms;

namespace Authentication.web.Pages
{
    public partial class Inscription
    {
        public SignUpModel model = new SignUpModel();
        public bool success;



        private void OnValidSubmit()
        {
            success = true;
            StateHasChanged();
        }
    }
}
