using System;
using System.Text;
using AutoMapper;
using GaiaProject.Common.Database;
using GaiaProject.Core.DataAccess;
using GaiaProject.Core.DataAccess.Abstractions;
using GaiaProject.Core.Logic;
using GaiaProject.Endpoint.Authentication;
using GaiaProject.Endpoint.Hubs;
using GaiaProject.Endpoint.Utils;
using GaiaProject.Endpoint.WorkerServices;
using GaiaProject.Engine.DataAccess;
using GaiaProject.Engine.DataAccess.Abstractions;
using GaiaProject.Engine.Logic;
using GaiaProject.Engine.Logic.Board.Map;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDbGenericRepository;
using MongoDbGenericRepository.Abstractions;
using Newtonsoft.Json;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Configure Azure Key Vault for production
if (builder.Environment.IsProduction())
{
    var azureServiceTokenProvider = new AzureServiceTokenProvider();
    var keyVaultClient = new KeyVaultClient(
        new KeyVaultClient.AuthenticationCallback(
            azureServiceTokenProvider.KeyVaultTokenCallback));

    builder.Configuration.AddAzureKeyVault(
        $"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/",
        keyVaultClient,
        new DefaultKeyVaultSecretManager());
}

// Add services to the container
builder.Services.AddMemoryCache();

// Configure the DB
ConfigureSupabase(builder.Services, builder.Configuration);

// Configure Authentication
ConfigureAuthentication(builder.Services, builder.Configuration);

builder.Services.AddSingleton<IClaimsTransformation, ApplicationUserFactory>();

builder.Services.AddLogging();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(opt =>
    {
        opt.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
        opt.SerializerSettings.MaxDepth = 1024;
    });

builder.Services.AddSignalR();
// TODO: Fix AutoMapper ambiguous call - there appear to be duplicate extension methods
// builder.Services.AddAutoMapper(cfg => cfg.AddProfile<GaiaProject.Endpoint.Mapping.Profiles>());

// Register application services
builder.Services.AddSingleton(builder.Services);
builder.Services.AddTransient<SupabaseUserDataProvider>();
builder.Services.AddTransient<IProvideUserData, SupabaseUserDataProvider>();
builder.Services.AddTransient<MongoGameDataProvider>();
builder.Services.AddTransient<CachedMongoGameDataProvider>();
builder.Services.AddTransient<IProvideGameData, CachedMongoGameDataProvider>();
builder.Services.AddSingleton(serviceProvider =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<ActiveGamesRegistry>>();
    return new ActiveGamesRegistry(logger);
});
builder.Services.AddSingleton(serviceProvider => new ActiveUsersRegistry());
builder.Services.AddTransient<GameManager>();
builder.Services.AddTransient<UserManager>();
builder.Services.AddTransient<GamesWorkerService>();
builder.Services.AddTransient(_ => new MapService(4, GaiaProject.Engine.Enums.MapShape.Standard4P));

// Configure SendGrid
//builder.Services.AddSendGrid(options =>
//{
//    options.ApiKey = builder.Configuration["SendGrid:ApiKey"];
//});
//builder.Services.AddTransient<MailService>();
//builder.Services.AddSingleton(new MailHelper(builder.Configuration["App:Urls:ReactFrontend"] ?? ""));

var app = builder.Build();

// TODO: Uncomment once AutoMapper issue is resolved
// var mapper = app.Services.GetRequiredService<IMapper>();
// mapper.ConfigurationProvider.AssertConfigurationIsValid();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors(config =>
{
    config.WithOrigins("https://localhost:3000", "https://192.168.1.*:3000", "capacitor://localhost", "http://localhost");
    config.AllowAnyHeader();
    config.AllowAnyMethod();
    config.AllowCredentials();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GaiaHub>("hubs/gaia");
app.MapControllers();

app.Run();

static void ConfigureSupabase(IServiceCollection services, IConfiguration configuration)
{
    var url = configuration["Database:ProjectUrl"] ?? throw new ArgumentNullException("Database:ProjectUrl is required");
    var key = configuration["Database:PublicApiKey"] ?? throw new ArgumentNullException("Database:PublicApiKey is required");
    var options = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = true
    };

    var supabase = new Supabase.Client(url, key, options);
    supabase.InitializeAsync();
    services.AddSingleton(supabase);
}

static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
{
    var bytes = Encoding.UTF8.GetBytes(
        configuration["Authentication:JwtSecret"] ?? throw new ArgumentNullException("Authentication:JwtSecret is required")
    );

    services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(bytes ?? throw new ArgumentNullException("Security key bytes missing")),
                ValidAudience = configuration["Authentication:ValidAudience"],
                ValidIssuer = configuration["Authentication:ValidIssuer"],
            };

            // options.Events = new JwtBearerEvents
            // {
            //     OnMessageReceived = context =>
            //     {
            //         // If the request is not for a hub, pass through
            //         var path = context.HttpContext.Request.Path;
            //         if (!path.StartsWithSegments("/hubs"))
            //         {
            //             return Task.CompletedTask;
            //         }

            //         var accessToken = context.Request.Query["access_token"];
            //         if (!string.IsNullOrEmpty(accessToken))
            //         {
            //             context.Token = accessToken;
            //         }
            //         return Task.CompletedTask;
            //     },
            // };
        });
    services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
}
