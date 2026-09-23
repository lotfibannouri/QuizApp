using Authentication.web.Model;
using Authentication.web.utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Security.Principal;
using Authentication.web.utility;
namespace Authentication.web.Pages
{

    public partial class Auth
    {
        [Inject] AuthenticationStateProvider AuthStateProvider { get; set; }
        public string role { get; set; }

        protected override async Task OnInitializedAsync()
        {
            
        }
        public Auth()
        {
            response.content = "";
            response.status = true;
        }

        LoginModel loginRequest = new LoginModel();
        Response response = new Response();

        //public async Task LogIn()
        //{
        //    Console.WriteLine("test");
        //    response = await service.LoginAsync(loginRequest);
        //    if (response.status)
        //    {

        //        if (response.content != null)
        //        {
        //            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        //            role = authState.User.FindFirst(ClaimTypes.Role)?.Value;
        //            if (role=="User")
        //            {
        //                NavigationManager.NavigateTo("/StDashBoard");
        //            }
        //            NavigationManager.NavigateTo("/Main?value=" + response.content);
        //        }
        //    }



        //}

        // ajouter ce using

public async Task LogIn()
    {
        response = await service.LoginAsync(loginRequest);
        if (response.status && response.content != null)
        {
            // Parser le rôle directement depuis le token — AVANT que l'AuthProvider soit notifié
            var claims = JwtParser.ParseClaimsFromJwt(response.content);
            role = claims.FirstOrDefault(c => c.Type == "aud")?.Value;

            if (role == "User")
                NavigationManager.NavigateTo("/StDashBoard");
            else
                NavigationManager.NavigateTo("/Main?value=" + response.content);
        }
    }

}
}
