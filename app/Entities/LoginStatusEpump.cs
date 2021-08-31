using System;

namespace app.Entities
{
    public class LoginStatusEpump
    {
        public int Id { get; set; }
        public bool IsLoggedIn { get; set; }

        public DateTime LoginDate { get; set; }
        public string EpumpDataId { get; set; }
        public virtual EpumpData EpumpData { get; set; }
    }
}