using Bot.Contracts.Enums;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;

namespace Bot.BackgroundWork.Services
{
    public partial class ReminderBot : SimpleTelegramBotBase
    {
        private readonly ILogger<ReminderBot> logger;
        private readonly IServiceScopeFactory scopeFactory;
        private ReminderCreationStatus status = ReminderCreationStatus.Empty;

        public ReminderBot(ILogger<ReminderBot> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;

            string? botToken = configuration.GetValue<string>("Telegram:BotToken");
            this.Client = new TelegramBotClient(botToken!);

            string? myUsername = this.Client.GetMe().Username!;

            this.Client.SetMyCommands([new BotCommand("help", "Справка по командам")]);
            this.Client.DeleteWebhook();

            // This will provide a better command filtering.
            this.SetCommandExtractor(myUsername);
        }

        public ITelegramBotClient Client { get; }
    }
}
