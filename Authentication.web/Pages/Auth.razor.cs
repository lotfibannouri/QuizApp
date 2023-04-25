using Authentication.web.Model;
using Microsoft.AspNetCore.Components;

namespace Authentication.web.Pages
{
    public partial class Auth
    {
        public Auth()
        {
            response.content = "";
            response.status = true;
        }

        LoginModel loginRequest = new LoginModel();
        Response response= new Response();

        public async Task LogIn()
        {
            Console.WriteLine("test");
            response = await service.LoginAsync(loginRequest);
            if(response.status)
            {
               
                if (response.content!= null) {
                    NavigationManager.NavigateTo("/Main?value="+response.content,true);
}
            }
           

        }
    }
}
