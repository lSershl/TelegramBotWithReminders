using Bot.Contracts.Dtos;

namespace Bot.Contracts.Entities
{
    public class Reminder
    {
        public int Id { get; set; }
        public required string ChatId { get; set; }
        public required string Description { get; set; }
        public DateTime ReminderDate { get; set; }

        public static ReminderDto AsDto(Reminder reminder)
        {
            return new ReminderDto(reminder.Id, reminder.ChatId, reminder.Description, reminder.ReminderDate.ToShortDateString());
        }
    }
}
