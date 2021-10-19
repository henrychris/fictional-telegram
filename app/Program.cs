using System;
using System.Threading.Tasks;
using app.Components;
using app.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace app
{
    public class Program
    {
        protected static DateTime ServerStart;
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            ServerStart = DateTime.Now;

            try
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (env == "Production")
                {
                    var context = services.GetRequiredService<DataContext>();
                    await context.Database.MigrateAsync();
                }
                else
                {
                    var context = services.GetRequiredService<PostGresDataContext>();
                    await context.Database.MigrateAsync();
                }

            }
            catch (Exception ex)
            {
                var errorHandler = services.GetRequiredService<ErrorHandler>();
                await errorHandler.HandleErrorAsync(ex);
                Console.WriteLine("An error occurred during migration.");
                Console.WriteLine(ex.Message);
            }

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
