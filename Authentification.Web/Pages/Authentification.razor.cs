using Authentification.Web.Model;
using Authentification.Web.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace Authentication.Web.Pages
{
    public partial class Authentification
    {
        public Authentification()
        {
            
        }
        
        LoginModel loginRequest = new LoginModel();

        
        public async Task LogIn()
        {
            var Response = await service.LoginAsync(loginRequest);
            Console.WriteLine(Response);

        }
    }
}
