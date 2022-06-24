using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDbGenericRepository;
using Newtonsoft.Json;
using SendGrid.Extensions.DependencyInjection;

namespace GaiaProject.Endpoint
{
	public class Startup
	{
		private IServiceCollection _services;
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			_services = services;

			services.AddMemoryCache();
			ConfigureDatabase(services);
			ConfigureAuthentication(services);

			services.AddSingleton<IClaimsTransformation, ApplicationUserFactory>();

			services.AddLogging();
			services
				.AddControllers()
				.AddNewtonsoftJson(opt =>
				{
					opt.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
					opt.SerializerSettings.MaxDepth = 1024;
				});
			services.AddSignalR();
			services.AddAutoMapper(GetType().Assembly);

			services.AddSingleton(services);
			services.AddTransient<MongoUserDataProvider>();
			services.AddTransient<CachedMongoUserDataProvider>();
			services.AddTransient<IProvideUserData, CachedMongoUserDataProvider>();
			services.AddTransient<MongoGameDataProvider>();
			services.AddTransient<CachedMongoGameDataProvider>();
			services.AddTransient<IProvideGameData, CachedMongoGameDataProvider>();
			services.AddSingleton(serviceProvider =>
			{
				var logger = serviceProvider.GetRequiredService<ILogger<ActiveGamesRegistry>>();
				return new ActiveGamesRegistry(logger);
			});
			services.AddSingleton(serviceProvider => new ActiveUsersRegistry());
			services.AddTransient<GameManager>();
			services.AddTransient<UserManager>();
			services.AddTransient<GamesWorkerService>();
			services.AddTransient(_ => new MapService(4, GaiaProject.Engine.Enums.MapShape.Standard4P));

			services.AddSendGrid(options =>
			{
				options.ApiKey = Configuration["SendGrid:ApiKey"];
			});
			services.AddTransient<MailService>();
			services.AddSingleton(new MailHelper(Configuration["App:Urls:ReactFrontend"]));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
		{
			mapper.ConfigurationProvider.AssertConfigurationIsValid();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseCors(config =>
			{
				config.WithOrigins("http://localhost:3000", "http://192.168.1.*:3000", "https://localhost:3000", "https://192.168.1.*:3000", "capacitor://localhost");
				config.AllowAnyHeader();
				config.AllowAnyMethod();
				config.AllowCredentials();
			});
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<GaiaHub>("hubs/gaia");
				endpoints.MapControllers();
			});
		}

		private void ConfigureDatabase(IServiceCollection services)
		{
			var connectionString = Configuration["AppSettings:MongoDbConnectionString"];
			var url = MongoUrl.Create(connectionString);
			var mongoClient = new MongoClient(url);
			var mongoDatabase = mongoClient.GetDatabase(url.DatabaseName);
			var mongoDbContext = new MongoDbContext(mongoDatabase);
			var readonlyRepo = new ReadOnlyMongoEntityRepository(mongoDbContext);
			var repo = new MongoEntityRepository(mongoDbContext);
			services.AddSingleton(mongoClient);
			services.AddSingleton(mongoDatabase);
			services.AddSingleton(readonlyRepo);
			services.AddSingleton(repo);
		}

		private void ConfigureAuthentication(IServiceCollection services)
		{
			string domain = $"https://{Configuration["Auth0:Domain"]}";

			services
				.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.Authority = domain;
					options.Audience = Configuration["Auth0:Audience"];
					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							// If the request is not for a hub, pass through
							var path = context.HttpContext.Request.Path;
							if (!path.StartsWithSegments("/hubs"))
							{
								return Task.CompletedTask;
							}

							var accessToken = context.Request.Query["access_token"];
							if (!string.IsNullOrEmpty(accessToken))
							{
								context.Token = accessToken;
							}
							return Task.CompletedTask;
						},
					};
				});

			services.AddAuthorization(options =>
			{
				options.AddPolicy("read:user", policy => policy.Requirements.Add(new HasScopeRequirement("read:user", domain)));
			});
			services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
		}
	}
}
