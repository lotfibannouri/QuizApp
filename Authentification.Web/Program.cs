using Authentication.Web;
using Authentification.Web.Service;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


builder.Services.AddHttpClient<IAuthentificationService, AuthentificationService>(client =>
{
#if (DEBUG)
    client.BaseAddress = new Uri("https://localhost:7297");
#else
	client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
#endif

});
await builder.Build().RunAsync();
