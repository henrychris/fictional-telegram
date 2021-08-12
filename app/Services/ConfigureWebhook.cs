using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace app.Services
{
    public class ConfigureWebhook : IHostedService
    {
        private readonly ILoggerManager _logger;
        private readonly IServiceProvider _services;
        private readonly BotConfiguration _botConfig;

        public ConfigureWebhook(ILoggerManager logger,
                                IServiceProvider services,
                                IConfiguration configuration)
        {
            _logger = logger;
            _services = services;
            _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Configure custom endpoint per Telegram API recommendations:
            // https://core.telegram.org/bots/api#setwebhook
            // If you'd like to make sure that the Webhook request comes from Telegram, we recommend
            // using a secret path in the URL, e.g. https://www.example.com/<token>.
            // Since nobody else knows your bot's token, you can be pretty sure it's us.
            // ! Code here is from Telegram.Bot.Examples.Webhook

            var webhookAddress = $"{_botConfig.WebhookUrl}/bot/{_botConfig.BotToken}";
            // Cancellation token isn't necessary
            var bot = await botClient.GetMeAsync(cancellationToken);
            _logger.LogInfo($"***\nSetting webhook:  {webhookAddress}\n***");

            try
            {
                await botClient.SetWebhookAsync(
                    url: webhookAddress,
                    allowedUpdates: Array.Empty<UpdateType>(),
                    cancellationToken: cancellationToken);

                _logger.LogInfo($"Webhook Started at {DateTime.Now:HH:mm:ss}\n");
                Console.WriteLine($"\n\t\tStarted listening for @{bot.Username} at {DateTime.Now:HH:mm:ss}\n");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Remove webhook upon app shutdown
            _logger.LogInfo("***\nRemoving webhook\n***");
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
    }
}