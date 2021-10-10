namespace app
{
    public class BotConfiguration
    {
        public string BotToken { get; init; }
        public string WebhookUrl { get; init; }
        public string DefaultConnection { get; init; }
        public string TelegramLoginUrl { get; init; }
        public string OTPUri { get; init; }
        public string EpumpReportUri { get; init; }
        public string EpumpApiUri { get; init; }
        public string AdminToken { get; init; }

    }
}