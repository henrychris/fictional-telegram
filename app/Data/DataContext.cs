using System.Data.Common;
using app.Entities;
using Microsoft.EntityFrameworkCore;

namespace app.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
    }
}