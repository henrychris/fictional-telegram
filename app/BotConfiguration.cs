namespace app
{
    public class BotConfiguration
    {
        public string BotToken { get; init; }
        public string WebhookUrl { get; init; }
        public string DefaultConnection { get; init; }
        public string TelegramLoginUrl { get; init; }
        public string EpumpLoginUrl { get; init; }
        public string EpumpReportUri { get; init; }
        public string EpumpApiUri { get; init; }

    }
}