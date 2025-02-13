using Bot.BackgroundWork.Calendar;
using Bot.Contracts.Calendar.Models;
using Bot.Contracts.Dtos;
using Bot.Contracts.Enums;
using Bot.Contracts.Repositories;
using Hangfire;
using Humanizer;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.UpdatingMessages;

namespace Bot.BackgroundWork.Services
{
    public partial class ReminderBot
    {
        private string tempDescription = string.Empty;
        private string tempDate = string.Empty;
        private int tempPreRemindDays = 1;

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
                        status = ReminderCreationStatus.DescriptionEntered;
                        string dateReply = "Укажите дату напоминания:";
                        this.Client.SendMessage(message.Chat.Id, dateReply);
                        var rm = new InlineKeyboardMarkup(CalendarInput.New(DateTime.Now));
                        this.Client.SendMessage(
                            message.Chat.Id,
                            "🗓 <b>Календарь</b> 🗓",
                            parseMode: FormatStyles.HTML,
                            replyMarkup: rm
                        );
                    }
                    break;
                case ReminderCreationStatus.DescriptionEntered:
                    string dateNotSetWarning = "Вы забыли указать дату в календаре!";
                    this.Client.SendMessage(message.Chat.Id, dateNotSetWarning);
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

                        string preReminderJobId = jobClient.Schedule(() => SendPreReminder(chatId, tempDescription, tempDate), preReminderOffset);
                        string reminderJobId = jobClient.Schedule(() => SendReminder(chatId, tempDescription, tempDate), reminderOffset);

                        repo.CreateReminderAsync(new ReminderDto(0, chatId, tempDescription, tempDate, preReminderJobId, reminderJobId));

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

        protected override void OnCallbackQuery(CallbackQuery query)
        {
            try
            {
                if (query.Message is null || string.IsNullOrEmpty(query.Data))
                {
                    return;
                }
                var queryArgs = query.Data.Split(' ');
                switch (queryArgs.ElementAt(0))
                {
                    case "месяц":
                        var month = new Month(
                            (MonthName)Enum.Parse(typeof(MonthName), queryArgs[2]),
                            uint.Parse(queryArgs[1])
                        );
                        this.Client.EditMessageReplyMarkup(
                            query.Message.Chat.Id,
                            query.Message.MessageId,
                            replyMarkup: new InlineKeyboardMarkup(CalendarInput.New(month))
                        );
                        break;
                    case "год":
                        this.Client.EditMessageReplyMarkup(
                            query.Message.Chat.Id,
                            query.Message.MessageId,
                            replyMarkup: new InlineKeyboardMarkup(CalendarInput.New(uint.Parse(queryArgs[1])))
                        );
                        break;
                    default:
                        this.Client.AnswerCallbackQuery(query.Id, query.Data, true);
                        tempDate = query.Data;
                        if (!string.IsNullOrEmpty(tempDate) && tempDate != "Пусто" && status == ReminderCreationStatus.DescriptionEntered)
                        {
                            status = ReminderCreationStatus.DateEntered;
                            string preRemindReply = "Укажите, за сколько дней начать предварительное напоминание:";
                            this.Client.SendMessage(query.Message.Chat.Id, preRemindReply);
                        }
                        break;
                }
            }
            catch (BotRequestException ex)
            {
                this.OnBotException(ex);
            }
            catch (Exception ex)
            {
                this.OnException(ex);
            }
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
