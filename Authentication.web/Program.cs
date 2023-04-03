using Authentication.web;
using Authentication.web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient <IAuthenticationService, AuthenticationService>(client =>
{
#if (DEBUG)
    client.BaseAddress = new Uri("https://localhost:7297");
#else
	client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
#endif

});

await builder.Build().RunAsync();
