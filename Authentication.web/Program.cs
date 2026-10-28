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

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();


builder.Services.AddAuthorizationCore();

builder.Services.AddLocalStorageServices();






var automapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = automapper.CreateMapper();
builder.Services.AddSingleton(mapper);

// Adresses des deux API, lues dans wwwroot/appsettings.json (production) et
// wwwroot/appsettings.Development.json (local). Ce fichier est public : jamais de secret ici.
static Uri ApiUrl(IConfiguration configuration, string key)
{
    var url = configuration[key];
    if (string.IsNullOrWhiteSpace(url))
        throw new InvalidOperationException($"Adresse d'API manquante : « {key} » dans wwwroot/appsettings.json.");
    return new Uri(url);
}

var authApiUrl = ApiUrl(builder.Configuration, "Api:AuthUrl");
var quizApiUrl = ApiUrl(builder.Configuration, "Api:QuizUrl");

void AddAuthApiClient<TClient, TImplementation>() where TClient : class where TImplementation : class, TClient
    => builder.Services.AddHttpClient<TClient, TImplementation>(client => client.BaseAddress = authApiUrl);

void AddQuizApiClient<TClient, TImplementation>() where TClient : class where TImplementation : class, TClient
    => builder.Services.AddHttpClient<TClient, TImplementation>(client => client.BaseAddress = quizApiUrl);

AddAuthApiClient<ICompteService, CompteService>();
AddAuthApiClient<IAdministrationService, AdministrationService>();

AddQuizApiClient<IQuizService, QuizService>();
AddQuizApiClient<ICategorieService, CategorieService>();
AddQuizApiClient<IQuestionService, QuestionService>();
AddQuizApiClient<IJdoodleService, JdoodleService>();
AddQuizApiClient<IReportService, ReportService>();
AddQuizApiClient<IProctoringService, ProctoringService>();
AddQuizApiClient<IResultatService, ResultatService>();

await builder.Build().RunAsync();
