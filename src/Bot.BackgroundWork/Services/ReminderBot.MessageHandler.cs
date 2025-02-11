using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI;
using Bot.Contracts.Enums;
using Telegram.BotAPI.AvailableMethods;
using Hangfire;
using Bot.Contracts.Repositories;
using Humanizer;
using Bot.Contracts.Dtos;

namespace Bot.BackgroundWork.Services
{
    public partial class ReminderBot
    {
        protected override void OnMessage(Message message)
        {
            using var scope = scopeFactory.CreateScope();

            // Ignore user 777000 (Telegram)
            if (message.From?.Id == TelegramConstants.TelegramId)
            {
                return;
            }

            bool hasText = !string.IsNullOrEmpty(message.Text); // True if message has text;

            switch (status)
            {
                case ReminderCreationStatus.CreationStarted:
                    if (hasText)
                    {
                        tempDescription = message.Text!;
                        string dateReply = "Напишите дату напоминания:";
                        this.Client.SendMessage(message.Chat.Id, dateReply);
                        status = ReminderCreationStatus.DescriptionEntered;
                    }
                    break;
                case ReminderCreationStatus.DescriptionEntered:
                    if (hasText)
                    {
                        tempDate = message.Text!;
                        string preRemindReply = "Укажите, за сколько дней начать предварительное напоминание:";
                        this.Client.SendMessage(message.Chat.Id, preRemindReply);
                        status = ReminderCreationStatus.DateEntered;
                    }
                    break;
                case ReminderCreationStatus.DateEntered:
                    if (hasText)
                    {
                        string chatId = message.Chat.Id.ToString();
                        tempPreRemindDays = int.Parse(message.Text!);
                        DateTime reminderDateTime = DateTime.Parse(tempDate).AtNoon().ToUniversalTime();
                        DateTimeOffset reminderOffset = new(reminderDateTime);
                        DateTimeOffset preReminderOffset = reminderOffset.AddDays(-tempPreRemindDays);

                        IBackgroundJobClient jobClient = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
                        IReminderRepository repo = scope.ServiceProvider.GetRequiredService<IReminderRepository>();

                        jobClient.Schedule(() => SendPreReminder(chatId, tempDescription, tempDate), preReminderOffset);
                        jobClient.Schedule(() => SendReminder(chatId, tempDescription, tempDate), reminderOffset);

                        repo.CreateReminderAsync(new ReminderDto(0, chatId, tempDescription, tempDate));

                        string reminderCompleteReply = "Напоминание создано";
                        this.Client.SendMessage(message.Chat.Id, reminderCompleteReply);
                        status = ReminderCreationStatus.CreationCompleted;
                    }
                    break;

            }
            #if DEBUG
            this.logger.LogInformation("New message from chat id: {ChatId}", message.Chat.Id);
            this.logger.LogInformation("Message: {MessageContent}", hasText ? message.Text : "No text");
            #endif

            base.OnMessage(message);
        }

        public void SendPreReminder(string chatId, string description, string date)
        {
            string preReminder = 
                $"Предварительно напоминаю вам о событии:\n" +
                $"Описание - {description}\n" +
                $"Дата - {date}";

            this.Client.SendMessage(chatId, preReminder);
        }

        public void SendReminder(string chatId, string description, string date)
        {
            string reminder =
                $"Сегодня у вас запланировано событие!\n" +
                $"Описание - {description}\n" +
                $"Дата - {date}";

            this.Client.SendMessage(chatId, reminder);
        }
    }
}
