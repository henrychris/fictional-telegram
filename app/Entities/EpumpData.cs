using System;

namespace app.Entities
{
    public class EpumpData
    {
        public string ID { get; set; }
        public long ChatId { get; set; } // foreign key
        public string CompanyId { get; set; }
        public string AuthKey { get; set; }
        public DateTime AuthDate { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public virtual AppUser User { get; set; }
    }
}