using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;

namespace Bot.BackgroundWork.Services
{
    public partial class ReminderBot
    {
        public override void OnUpdate(Update update)
        {
            #if DEBUG
            this.logger.LogInformation(
                "New update with id: {UpdateId}. Type: {UpdateType}",
                update.UpdateId,
                update.GetUpdateType()
            );
            #endif

            base.OnUpdate(update);
        }
    }
}
