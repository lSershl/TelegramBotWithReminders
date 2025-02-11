using Bot.Contracts.Dtos;
using Bot.Contracts.Entities;

namespace Bot.Contracts.Repositories
{
    public interface IReminderRepository
    {
        Task<List<ReminderDto>> GetRemindersAsync(string chatId);
        void CreateReminderAsync(ReminderDto reminderDto);
        void RemoveOutdatedRemindersAsync(List<Reminder> remindersList);
    }
}
