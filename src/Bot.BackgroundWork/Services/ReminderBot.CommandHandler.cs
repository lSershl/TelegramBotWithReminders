using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Bot.Contracts.Repositories;
using System.Text;

namespace Bot.BackgroundWork.Services
{
    public partial class ReminderBot
    {
        protected override void OnCommand(Message message, string commandName, string commandParameters)
        {
            string[] args = commandParameters.Split(' ');
            #if DEBUG
            this.logger.LogInformation("Params: {ArgsLength}", args.Length);
            #endif

            using var scope = scopeFactory.CreateScope();

            switch (commandName)
            {
                case "напомнить":
                    string reply = $"Давайте создадим напоминание.\n" +
                        $"Напишите краткое описание напоминания:";
                    status = Contracts.Enums.ReminderCreationStatus.CreationStarted;
                    this.Client.SendMessage(message.Chat.Id, reply);
                    break;
                case "список":
                    var repo = scope.ServiceProvider.GetRequiredService<IReminderRepository>();
                    var remindersList = repo.GetRemindersAsync(message.Chat.Id.ToString()).Result;
                    if (remindersList.Count > 0)
                    {
                        int count = 1;
                        StringBuilder sb = new();
                        foreach (var item in remindersList)
                        {
                            sb.Append($"{count}. Описание: {item.Description}. Дата: {item.ReminderDate}\n");
                            count++;
                        }
                        this.Client.SendMessage(message.Chat.Id, sb.ToString());
                    }
                    else
                    {
                        this.Client.SendMessage(message.Chat.Id, "Ваш список напоминаний пуст");
                    }
                    break;
                default:
                    if (message.Chat.Type == ChatTypes.Private)
                    {
                        this.Client.SendMessage(message.Chat.Id, "Неизвестная команда");
                    }
                    break;
            }
        }
    }
}
