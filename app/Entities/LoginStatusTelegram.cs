using System;

namespace app.Entities
{
    public class LoginStatusTelegram
    {
        public int Id { get; set; }
        public bool IsLoggedIn { get; set; }
        public DateTime LoginDate { get; set; }
        public long UserChatId { get; set; }
        public virtual AppUser User { get; set; }
    }
}