using System;
using System.IO;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NLog;
using Telegram.Bot;
using app.Components;
using app.Components.Reports;
using app.Data;
using app.Data.Repository;
using app.Interfaces;
using app.Middleware;
using app.Services;

namespace app
{

    public static class ServicesExtensions
    {

        public static void AddComponents(this IServiceCollection services)
        {
            services.AddTransient<Keyboards>();
            services.AddScoped<ErrorHandler>();

            services.AddTransient<BranchReportsKeyboards>();
            services.AddTransient<BranchReports>();
            services.AddTransient<CompanyReports>();
            services.AddTransient<CompanyReportsKeyboards>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
        }

        public static void ConfigureDbContext(this IServiceCollection services, string dbConnectionString)
        {
            services.AddDbContext<DataContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    // Use connection string from file.
                    connStr = dbConnectionString;
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];
                    // SslMode = SslMode.Require, TrustServerCertificate = true

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;TrustServerCertificate=True;";
                }

                // Whether the connection string came from the local development configuration file
                // or from the environment variable from Heroku, use it to set up your DbContext.
                options.UseNpgsql(connStr);
            });
        }
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            BotConfig = Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }
        private BotConfiguration BotConfig { get; }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors((builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                var token = BotConfig.BotToken;
                endpoints.MapControllers();

                endpoints.MapControllerRoute(name: "tgwebhook",
                                            pattern: $"bot/{token}",
                                            new { controller = "Webhook", action = "Post" });
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Telegram.Bot uses JSON.NET to deserialize incoming messages.
            services.AddControllers().AddNewtonsoftJson();

            services.AddHostedService<ConfigureWebhook>();

            services.AddScoped<HandleUpdateService>();

            services.AddHttpClient("tgwebhook")
                    .AddTypedClient<ITelegramBotClient>(httpClient
                        => new TelegramBotClient(BotConfig.BotToken, httpClient));

            services.AddHttpClient("EpumpReportApi", client =>
            {
                client.BaseAddress = new Uri(BotConfig.EpumpReportUri);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient("TestApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient("EpumpApi", client =>
            {
                client.BaseAddress = new Uri(BotConfig.EpumpApiUri);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEpumpDataRepository, EpumpDataRepository>();
            services.AddScoped<ILoginStatusRepository, LoginStatusMethods>();

            services.ConfigureLoggerService();
            services.ConfigureAutoMapper();
            services.ConfigureDbContext(BotConfig.DefaultConnection);

            services.AddComponents();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "app", Version = "v1" });
            });
        }
    }
}
