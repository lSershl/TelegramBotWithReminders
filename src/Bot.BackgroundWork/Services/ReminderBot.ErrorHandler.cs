using Telegram.BotAPI;

namespace Bot.BackgroundWork.Services
{
    public partial class ReminderBot
    {
        protected override void OnBotException(BotRequestException exp) =>
            this.logger.LogError("BotRequestException: {Message}", exp.Message);

        protected override void OnException(Exception exp) =>
            this.logger.LogError("Exception: {Message}", exp.Message);
    }
}
