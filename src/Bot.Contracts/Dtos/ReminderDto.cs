namespace Bot.Contracts.Dtos
{
    public record ReminderDto(int Id, string ChatId, string Description, string ReminderDate, string PreReminderJobId, string ReminderJobId);
}
