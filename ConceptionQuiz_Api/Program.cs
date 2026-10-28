using AutoMapper;
using ConceptionQuiz_Api.Metier;
using ConceptionQuiz_Api.Models;
using ConceptionQuiz_Api.Repository;
using ConceptionQuiz_Api.utility;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Relance automatique : une base Azure SQL serverless en pause refuse la première connexion
// le temps de se réveiller.
builder.Services.AddDbContext<ConceptionQuizDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    sql => sql.EnableRetryOnFailure()));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var automapper = new MapperConfiguration(item => item.AddProfile(new AutoMapperHandler()));
IMapper mapper = automapper.CreateMapper();
builder.Services.AddSingleton(mapper);

// Seul le site lui-même peut appeler l'API depuis un navigateur (adresses dans Cors:AllowedOrigins).
// AllowCredentials est exigé par SignalR (hub /notificationshub).
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    }
        );
}
);
builder.Services.AddHttpClient();
builder.Services.AddTransient<IQuizRepository, QuizRepository>();
builder.Services.AddTransient<IQuestionRepository, QuestionRepository>();
builder.Services.AddTransient<ICategorieRepository, CategorieRepository>();
builder.Services.AddTransient<IProctoringRepository, ProctoringRepository>();
builder.Services.AddTransient<IResultatRepository, ResultatRepository>();
builder.Services.AddHostedService<ProctoringRetentionService>();
builder.Services.AddScoped<IQuizReport, QuizReport>();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddSignalR();
QuestPDF.Settings.License = LicenseType.Community;
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();


app.MapHub<NotificationsHub>("/notificationshub");

app.Run();
