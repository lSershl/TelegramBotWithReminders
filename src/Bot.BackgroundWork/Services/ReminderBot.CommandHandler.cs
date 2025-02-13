using Bot.Contracts.Repositories;
using Hangfire;
using System.Text;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

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

            using IServiceScope scope = scopeFactory.CreateScope();
            IReminderRepository repo = scope.ServiceProvider.GetRequiredService<IReminderRepository>();
            IBackgroundJobClient jobClient = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();

            switch (commandName)
            {
                case "help":
                    string helpReply = "Здравствуйте. Вот справка по командам:\n" +
                        "/напомнить - запускает процесс создания напоминания, следуйте инструкциям от бота\n" +
                        "/список - запрашивает список созданных напоминаний с описанием и датой (отсортирован по возрастанию даты)\n" +
                        "/очистить - удаляет все напоминания и предварительные напоминания";
                    this.Client.SendMessage(message.Chat.Id, helpReply);
                    break;
                case "напомнить":
                    string startReminderCreationReply = $"Давайте создадим напоминание.\n" +
                        $"Напишите краткое описание напоминания:";
                    status = Contracts.Enums.ReminderCreationStatus.CreationStarted;
                    this.Client.SendMessage(message.Chat.Id, startReminderCreationReply);
                    break;
                case "список":
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
                case "очистить":
                    var remindersToDeleteList = repo.GetRemindersAsync(message.Chat.Id.ToString()).Result;
                    foreach (var item in remindersToDeleteList)
                    {
                        jobClient.Delete(item.PreReminderJobId);
                        jobClient.Delete(item.ReminderJobId);
                    }
                    repo.ClearAllRemindersAsync(message.Chat.Id.ToString());
                    string remindersClearedReply = "Список напоминаний очищен";
                    this.Client.SendMessage(message.Chat.Id, remindersClearedReply);
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
