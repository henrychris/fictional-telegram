using System;
using System.IO;
using app.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace app.Data
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
                var connectionString = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();

                string connStr = connectionString.DefaultConnection;
                optionsBuilder.UseSqlServer(connStr);
            };
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<EpumpData> EpumpData { get; set; }
    }
}