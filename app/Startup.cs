using System;
using System.IO;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NLog;
using Telegram.Bot;
using app.Components;
using app.Services;
using app.Data;
using Microsoft.EntityFrameworkCore;
using app.Interfaces;
using app.Data.Repository;
using app.Components.Reports;

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
        }

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
        }
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureDbContext(this IServiceCollection services, string dbConnectionString)
        {
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(dbConnectionString);
            });
        }
    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            LogManager.LoadConfiguration("app/nlog.config");
            BotConfig = Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }

        private IConfiguration Configuration { get; }
        private BotConfiguration BotConfig { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "app v1"));
            }

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
