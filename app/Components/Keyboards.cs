using System;
using System.Threading.Tasks;
using app.Interfaces;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace app.Components
{
    public class Keyboards
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ErrorHandler _errorHandler;
        private readonly IUserRepository _userRepository;
        private readonly BotConfiguration _botConfig;

        public Keyboards(ITelegramBotClient botClient, ErrorHandler errorHandler,
        IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _errorHandler = errorHandler;
            _botClient = botClient;
            _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }
        public async Task<Message> Start(Message message)
        {
            var chatDetails = message.Chat;
            var userCheck = await _userRepository.CheckUserExistsAsync(message.Chat.Id);
            try
            {
                if (userCheck)
                {
                    var response = await _botClient.SendTextMessageAsync(message.Chat.Id, $"Hello, {chatDetails.FirstName}!\nUse /menu to get started",
                        replyMarkup: new ReplyKeyboardRemove());
                    return response;
                }
            }
            catch (Exception exception)
            {
                await _errorHandler.HandleErrorAsync(exception);
            }

            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Hello, you are currently not logged in!\nUse /menu to get started.");
        }

        public async Task<Message> SendMenu(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            InlineKeyboardMarkup menuKeyboard = new(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Login"),
                    InlineKeyboardButton.WithCallbackData("Reports")
                },
            });
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Welcome to Epump!", replyMarkup: menuKeyboard);
        }

        public async Task<Message> SendHelp(Message message)
        {
            var response = @"I can help you view various reports, and keep you up to date with notifications!

You can control me with these commands:
/start - start/restart me
/menu - display the menu";

            return await _botClient.SendTextMessageAsync(message.Chat.Id, response);
        }

        public async Task<Message> SendLoginKeyboard(Message message)
        {
            // user check to prevent multiple logins
            var chatDetails = message.Chat;
            var userCheck = await _userRepository.CheckUserExistsAsync(message.Chat.Id);
            try
            {
                if (userCheck)
                {
                    await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                    return await _botClient.SendTextMessageAsync(message.Chat.Id, "You are already logged in!\nUse /menu to get started",
                        replyMarkup: new ReplyKeyboardRemove());
                }
            }
            catch (Exception exception)
            {
                await _errorHandler.HandleErrorAsync(exception);
            }

            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            InlineKeyboardMarkup loginKeyboard = new(new[]
            {
                new []
                {
                    // url to login page.
                    InlineKeyboardButton.WithUrl("Login", _botConfig.LoginUrl)
                    // InlineKeyboardButton.WithUrl("Open Login Page", "https://cutt.ly/YmHIFok")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Back","Menu")
                }
            });
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Login to Epump", replyMarkup: loginKeyboard);
        }

        public async Task<Message> SendReportKeyboard(Message message)
        {
            // add check to prevent users who aren't in the Db from accessing this.
            if (await _userRepository.CheckUserExistsAsync(message.Chat.Id))
            {
                await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                InlineKeyboardMarkup reportKeyboard = new(new[]
                {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Branch", "BranchReports"),
                    InlineKeyboardButton.WithCallbackData("Company", "CompanyReports"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Back", "Menu")
                }
            });
                return await _botClient.SendTextMessageAsync(message.Chat.Id, "Choose a Report", replyMarkup: reportKeyboard);
            }

            else 
            {
                return await _botClient.SendTextMessageAsync(message.Chat.Id, "You are not logged in!\nUse /menu to get started");
            }
        }

        public async Task<Message> SendCompanyReportsKeyboard(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            InlineKeyboardMarkup companyReportKeyboard = new(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Branch Sales"),
                    InlineKeyboardButton.WithCallbackData("Company Cashflow")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Sales Summary"),
                    InlineKeyboardButton.WithCallbackData("Tank Stock")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Company Tanks Filled"),
                    InlineKeyboardButton.WithCallbackData("Company Variance")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Expense Categories"),
                    InlineKeyboardButton.WithCallbackData("Zones Report")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Outstanding Payments"),
                    InlineKeyboardButton.WithCallbackData("Wallet Report")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Retainership"),
                    InlineKeyboardButton.WithCallbackData("Wallet Fund Request")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Back", "Reports")
                }
            });
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Company-Level Reports", replyMarkup: companyReportKeyboard);
        }

        public async Task<Message> SendBranchReportsKeyboard(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            InlineKeyboardMarkup ReportKeyboard = new(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Cashflow"),
                    InlineKeyboardButton.WithCallbackData("Product Summary")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Sales Transactions"),
                    InlineKeyboardButton.WithCallbackData("Tanks Filled")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Variance"),
                    InlineKeyboardButton.WithCallbackData("Tank Report")
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Back", "Reports")
                }
            });
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Branch-Level Reports", replyMarkup: ReportKeyboard);
        }
    }
}