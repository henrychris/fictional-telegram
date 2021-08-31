using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tests.Mocks.Data
{
    public class MockContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<EpumpData> EpumpData { get; set; }
        public DbSet<LoginStatusTelegram> loginStatusTelegram { get; set; }
        public DbSet<LoginStatusEpump> loginStatusEpump { get; set; }
    }

    public class AppUser
    {
        [Key]
        [Required]
        public long ChatId { get; set; }
        [Required] public string FirstName { get; set; }
        public string Username { get; set; }
        public DateTime AuthDate { get; set; }
        public string Hash { get; set; }
        public string State { get; set; }
        public string CurrentBranch { get; set; }

        public string EpumpDataId { get; set; }
        public virtual EpumpData EpumpData { get; set; }
    }

    public class EpumpData
    {
        public string ID { get; set; }
        public long ChatId { get; set; } // foreign key
        public string CompanyId { get; set; }
        public string AuthKey { get; set; }
        public string Role { get; set; }
        public virtual AppUser User { get; set; }
    }

    public class LoginStatusTelegram
    {
        public int Id { get; set; }
        public bool IsLoggedIn { get; set; }
        public DateTime LoginDate { get; set; }
        public long UserChatId { get; set; }
        public virtual AppUser User { get; set; }
    }

    public class LoginStatusEpump
    {
        public int Id { get; set; }
        public bool IsLoggedIn { get; set; }

        public DateTime LoginDate { get; set; }
        public string EpumpDataId { get; set; }
        public virtual EpumpData EpumpData { get; set; }
    }
}