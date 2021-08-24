using System;
using System.ComponentModel.DataAnnotations;

namespace app.Entities
{
    public class LoginStatus
    {
        public int Id { get; set; }
        public bool isLoggedInTelegram { get; set; }
        public bool isLoggedInEpump { get; set; }

        public DateTime loginDateTelegram { get; set; }
        public DateTime loginDateEpump { get; set; }

        public long UserChatId { get; set; }
        public virtual AppUser User { get; set; }

        public string EpumpDataId { get; set; }
        public virtual EpumpData EpumpData { get; set; }
    }
}