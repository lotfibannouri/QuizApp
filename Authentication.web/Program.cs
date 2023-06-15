using Authentication.web;
using Authentication.web.AuthProviders;
using Authentication.web.AutoMapper_Handler;
using Authentication.web.Services;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor.Services;
using Microsoft.AspNetCore.Authorization.Policy;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();

builder.Services.AddAuthorizationCore();

builder.Services.AddLocalStorageServices();


//builder.Services.AddSignalRCore();


var automapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = automapper.CreateMapper();
builder.Services.AddSingleton(mapper);

//builder.Services.AddSingleton(sp =>
//{
//    var navigationManager = sp.GetRequiredService<NavigationManager>();
//    return new HubConnectionBuilder()
//        .WithUrl(navigationManager.ToAbsoluteUri("/notificationHub"))
//        .Build();
//});

builder.Services.AddHttpClient <ICompteService, CompteService>(client =>
{
#if (DEBUG)
    client.BaseAddress = new Uri("https://localhost:7297");
#else
	client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
#endif

});
builder.Services.AddHttpClient<IAdministrationService, AdministrationService>(client =>
{
#if (DEBUG)
    client.BaseAddress = new Uri("https://localhost:7297");
#else
	client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
#endif

});

builder.Services.AddHttpClient<IQuizService,QuizService>(client =>
{
#if (DEBUG)
    client.BaseAddress = new Uri("https://localhost:7284");
#else
	client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
#endif

});

builder.Services.AddHttpClient<IQuestionService, QuestionService>(client =>
{
#if (DEBUG)
    client.BaseAddress = new Uri("https://localhost:7284");
#else
	client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
#endif

});

await builder.Build().RunAsync();
