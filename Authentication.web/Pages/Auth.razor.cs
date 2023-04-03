using Authentication.web.Model;

namespace Authentication.web.Pages
{
    public partial class Auth
    {
        public Auth()
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
