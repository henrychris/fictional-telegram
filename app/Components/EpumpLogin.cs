using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using app.Controllers.Helper;
using app.Data.DTOs;
using app.Interfaces;
using app.Errors;
using app.Data;

namespace app.Components
{
    public class EpumpLogin : IEpumpLogin
    {
        private readonly ITelegramBotClient _botClient;
        private readonly BotControllerHelper _botControllerHelper;
        private readonly IUserRepository _userRepository;
        private readonly HttpClient _client;
        private BotConfiguration BotConfiguration { get; }

        public EpumpLogin(ITelegramBotClient botClient, IUserRepository userRepository,
        IHttpClientFactory httpClientFactory, BotControllerHelper botControllerHelper
        , IConfiguration configuration)
        {
            _botControllerHelper = botControllerHelper;
            _userRepository = userRepository;
            _botClient = botClient;
            _client = httpClientFactory.CreateClient("OTPUri");

            BotConfiguration = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }

        public async Task SendEpumpLoginKeyboard(Message message)
        {
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            await _userRepository.SetUserStateAsync(message.Chat.Id, "EpumpLogin_InputEmail");
            await _botClient.SendTextMessageAsync(message.Chat.Id, "Please input your Epump email", replyMarkup: new ForceReplyMarkup());
        }

        public bool ValidateEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Message> PromptUserForOtp(Message message)
        {
            var email = message.Text;
            var isValidEmail = ValidateEmail(email);
            await _userRepository.SetUserEmail(message.Chat.Id, email);

            if (!isValidEmail)
            {
                return await _botClient.SendTextMessageAsync(message.Chat.Id, "Please input a valid email", replyMarkup: new ForceReplyMarkup());
            }

            await _botClient.SendTextMessageAsync(message.Chat.Id, "Validating email... Please wait.");

            // send request for verification to API
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {BotConfiguration.AdminToken}");
            await _client.GetAsync($"Account/TelegramOTP?email={email}&chatId={message.Chat.Id}");

            await _userRepository.SetUserStateAsync(message.Chat.Id, "EpumpLogin_ValidateOTP");
            return await _botClient.SendTextMessageAsync(message.Chat.Id, $"Please input the OTP sent to {email}", replyMarkup: new ForceReplyMarkup());
        }

        public async Task<Message> ReceiveOtpFromUserInput(Message message)
        {
            var otp = message.Text;
            var isValidOtp = await CheckIfOtpIsValid(message.Chat.Id, otp);

            if (isValidOtp)
                return await _botClient.SendTextMessageAsync(message.Chat.Id,
                    "Success!\nWelcome to Epump. \nUse /menu to get started");

            // maybe remove the message below?
            await _botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            return await _botClient.SendTextMessageAsync(message.Chat.Id, "An error occurred.");
        }

        private async Task<bool> CheckIfOtpIsValid(long chatId, string otp)
        {
            var email = await _userRepository.GetUserEmail(chatId);
            var response = await SendVerificationRequest(otp, email, chatId);

            if (!response.IsSuccessStatusCode)
            {
                await HandleError(chatId, response);
                return false;
            }

            var content = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<EpumpDataDto>(content);

            return await _botControllerHelper.RegisterEpumpUser(result);
        }

        private async Task<HttpResponseMessage> SendVerificationRequest(string otp, string email, long chatId)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {BotConfiguration.AdminToken}");
            await _botClient.SendTextMessageAsync(chatId, "Verifying OTP... Please wait.");
            var request = new HttpRequestMessage(HttpMethod.Post, $"Account/VerifyTelegramOTP?OTP={otp}&email={email}");

            var response = await _client.SendAsync(request);
            return response;
        }

        private async Task HandleError(long chatId, HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await _botClient.SendTextMessageAsync(chatId, "The OTP was incorrect or you may not be authorized to use this" +
                " application.");
                return;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                await _botClient.SendTextMessageAsync(chatId, "You do not have the required roles to use this application.");
                return;
            }
            else
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                EpumpError result = await JsonSerializer.DeserializeAsync<EpumpError>(contentStream);
                await _botClient.SendTextMessageAsync(chatId, result.message);
                return;
            }
        }
    }
}