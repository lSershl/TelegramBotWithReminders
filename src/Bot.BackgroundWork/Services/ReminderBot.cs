using Telegram.BotAPI.Extensions;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Bot.Contracts.Enums;

namespace Bot.BackgroundWork.Services
{
    public partial class ReminderBot : SimpleTelegramBotBase
    {
        private readonly ILogger<ReminderBot> logger;
        private readonly IServiceScopeFactory scopeFactory;
        private ReminderCreationStatus status;

        private string tempDescription = string.Empty;
        private string tempDate = string.Empty;
        private int tempPreRemindDays = 1;

        public ReminderBot(ILogger<ReminderBot> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;

            string? botToken = configuration.GetValue<string>("Telegram:BotToken");
            this.Client = new TelegramBotClient(botToken!);

            string? myUsername = this.Client.GetMe().Username!;
            // This will provide a better command filtering.
            this.SetCommandExtractor(myUsername);
        }

        public ITelegramBotClient Client { get; }
    }
}
