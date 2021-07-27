namespace app.Data.DTOs
{
    public class TelegramUserDto
    {
        // used for verifying hash
        // naming conventions are changed to match data received from telegram
        public long id { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string photo_url { get; set; }

        public long auth_date { get; set; }

        public string hash { get; set; }

        public string auth_key { get; set; }
    }
}