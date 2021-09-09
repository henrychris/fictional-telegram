using System.Collections.Generic;

namespace app.Data.DTOs
{
    public class EpumpUser
    {
        public string id { get; set; }
        public string companyId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public List<string> roles { get; set; }
        public string chatId { get; set; }
        public string companyName { get; set; }
    }
}