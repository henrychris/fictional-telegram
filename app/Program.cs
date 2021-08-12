using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace app
{
    public class Program
    {
        protected static DateTime ServerStart;
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            ServerStart = DateTime.Now;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    // webBuilder.UseUrls("http://localhost:80");
                });
    }
}
