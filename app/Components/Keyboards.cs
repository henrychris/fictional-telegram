using System.Threading.Tasks;
using app.Interfaces;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace app.Components
{
    public class Keyboards
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserRepository _userRepository;
        private readonly BotConfiguration _botConfig;
        private readonly IEpumpDataRepository _epumpDataRepository;
        private InlineKeyboardMarkup _loginKeyboard;

        public Keyboards(ITelegramBotClient botClient,
        IUserRepository userRepository, IConfiguration configuration,
        IEpumpDataRepository epumpDataRepository)
        {
            _epumpDataRepository = epumpDataRepository;
            _userRepository = userRepository;
            _botClient = botClient;
            _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }
        public async Task<Message> Start(Message message)
        {
            var chatDetails = message.Chat;
            var userCheck = await CheckForUserInDb(message);
            var loginStage = "";

            if (userCheck)
            {
                loginStage = "all";
            }
            if (!await _userRepository.CheckUserExistsAsync(message.Chat.Id))
            {
                loginStage = "none";
            }
            if (await _userRepository.CheckUserExistsAsync(message.Chat.Id) && !await _epumpDataRepository.CheckForChatIdAsync(message.Chat.Id))
            {
                loginStage = "telegram";
            }

            return (loginStage) switch
            {
                "all" => await _botClient.SendTextMessageAsync(message.Chat.Id, $"Hello, {chatDetails.FirstName}!\nUse /menu to get started",
                    replyMarkup: new ReplyKeyboardRemove()),
                "none" => await _botClient.SendTextMessageAsync(message.Chat.Id, "Hello, you are currently not logged in!\nUse /menu to get started."),
                "telegram" => await _botClient.SendTextMessageAsync(message.Chat.Id, "Hello, you are not logged in with Epump\nUse /menu to get started."),
                _ => await _botClient.SendTextMessageAsync(message.Chat.Id, "Hello, you are currently not logged in!\nUse /menu to get started."),
            };
        }

        public async Task<Message> SendConfirmationKeyboard(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            // check if user is logged in
            var userCheck = await CheckForUserInDb(message);
            if (!userCheck)
            {
                return await _botClient.SendTextMessageAsync(message.Chat.Id, "You are not logged in!\nUse /menu to get started.");
            }

            InlineKeyboardMarkup keyboard = new(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Delete My Data", "DeleteUserData"),
                }
                , new []
                {
                    InlineKeyboardButton.WithCallbackData("Back", "Menu")
                }
            });
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Doing this will PERMANENTLY delete your data.\nYou will have to log back in to use this bot again.", replyMarkup: keyboard);

        }

        public async Task DeleteUserData(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

            await _userRepository.DeleteUser(message.Chat.Id);
            var user = await _epumpDataRepository.GetUserDetailsAsync(message.Chat.Id);
            await _epumpDataRepository.DeleteUserAsync(user.ID);
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Your data has been deleted successfully", replyMarkup: new ReplyKeyboardRemove());
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
                }
            });
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Welcome to Epump!", replyMarkup: menuKeyboard);
        }

        public async Task<Message> SendHelp(Message message)
        {
            var response = @"I can help you view various reports, and keep you up to date with notifications!

You can control me with these commands:
/start - start/restart me
/menu - display the menu
/reports - retrieve a report
/delete - permanently delete your data";

            return await _botClient.SendTextMessageAsync(message.Chat.Id, response);
        }

        public async Task<Message> SendLoginKeyboard(Message message)
        {
            // Check if user has not authorized with Telegram. 
            if (!await _userRepository.CheckUserExistsAsync(message.Chat.Id))
            {
                await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                _loginKeyboard = new(new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Login", _botConfig.TelegramLoginUrl),
                    },
                    new[]
                    {
                            InlineKeyboardButton.WithCallbackData("Back", "Menu"),
                    }
                });

                return await _botClient.SendTextMessageAsync(message.Chat.Id, "Make a Selection"
                    , replyMarkup: _loginKeyboard, parseMode: ParseMode.Markdown);
            }

            // Checks if user has authorized with Telegram, but not with Epump.
            else if (await _userRepository.CheckUserExistsAsync(message.Chat.Id) && !await _epumpDataRepository.CheckForChatIdAsync(message.Chat.Id))
            {
                await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);

                _loginKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Login With Epump", "EpumpLogin"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Back", "Menu"),
                    }
                });

                return await _botClient.SendTextMessageAsync(message.Chat.Id, "Make a Selection", replyMarkup: _loginKeyboard, parseMode: ParseMode.Markdown);
            }

            // Else the user must be completely logged in.
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "You are already logged in!\nUse /menu to get started",
                    replyMarkup: new ReplyKeyboardRemove());
        }

        private async Task<bool> CheckForUserInDb(Message message)
        {
            // user check to prevent multiple logins
            return await _userRepository.CheckUserExistsAsync(message.Chat.Id)
            && await _epumpDataRepository.CheckForChatIdAsync(message.Chat.Id);
        }

        public async Task<Message> SendReportKeyboard(Message message)
        {
            // add check to prevent users who aren't in the Db from accessing this.
            if (await CheckForUserInDb(message))
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
                    InlineKeyboardButton.WithCallbackData("Company POS Transactions"),
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
            InlineKeyboardMarkup reportKeyboard = new(new[]
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
                    InlineKeyboardButton.WithCallbackData("Branch POS Transactions"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData("Back", "Reports")
                }
            });
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "Branch-Level Reports", replyMarkup: reportKeyboard);
        }
    }
}