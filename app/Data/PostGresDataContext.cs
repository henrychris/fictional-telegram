using app.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace app.Data
{
    public class PostGresDataContext : DbContext
    {
        public PostGresDataContext()
        {

        }

        public PostGresDataContext(DbContextOptions<PostGresDataContext> options) : base(options)
        {
        }        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Development.json")
                .Build();
                var connectionString = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();

                string connStr = connectionString.DefaultConnection;

                optionsBuilder.UseNpgsql(connStr);
            };
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<EpumpData> EpumpData { get; set; }
    }
}
