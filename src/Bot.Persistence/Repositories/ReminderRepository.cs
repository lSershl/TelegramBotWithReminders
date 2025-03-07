﻿using Bot.Contracts.Dtos;
using Bot.Contracts.Entities;
using Bot.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using Humanizer;

namespace Bot.Persistence.Repositories
{
    public class ReminderRepository(RemindersDbContext dbContext) : IReminderRepository
    {
        private readonly RemindersDbContext db = dbContext;
        public async Task<List<ReminderDto>> GetRemindersAsync(string chatId)
        {
            var remindersList = await db.Reminders
                .Where(x => x.ChatId == chatId)
                .OrderBy(x => x.ReminderDate)
                .ToListAsync();
            DeleteOutdatedRemindersAsync(remindersList);
            var result = new List<ReminderDto>();
            foreach (var item in remindersList)
            {
                result.Add(Reminder.AsDto(item));
            }
            return result;
        }

        public async void CreateReminderAsync(ReminderDto reminderDto)
        {
            db.Add(new Reminder
            {
                Id = reminderDto.Id,
                ChatId = reminderDto.ChatId,
                Description = reminderDto.Description,
                ReminderDate = DateTime.Parse(reminderDto.ReminderDate).AtNoon().ToUniversalTime(),
                PreReminderJobId = reminderDto.PreReminderJobId,
                ReminderJobId = reminderDto.ReminderJobId
            });
            await db.SaveChangesAsync();
        }

        public async void DeleteOutdatedRemindersAsync(List<Reminder> remindersList)
        {
            foreach (var item in remindersList)
            {
                if (item.ReminderDate < DateTime.UtcNow)
                {
                    db.Remove(item);
                }
            }
            await db.SaveChangesAsync();
        }

        public async void ClearAllRemindersAsync(string chatId)
        {
            var remindersList = await db.Reminders.Where(x => x.ChatId == chatId).ToListAsync();
            db.Reminders.RemoveRange(remindersList);
            await db.SaveChangesAsync();
        }
    }
}
