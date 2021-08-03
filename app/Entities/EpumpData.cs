namespace app.Entities
{
    public class EpumpData
    {
        public string ID { get; set; }
        public long ChatId { get; set; } // foreign key
        public string CompanyId { get; set; } // foreign key
        public string AuthKey { get; set; }
        public string Role { get; set; }
        public virtual AppUser User { get; set; }
        public virtual Company Company { get; set; }
    }
}