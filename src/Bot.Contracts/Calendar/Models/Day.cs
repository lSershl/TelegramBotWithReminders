namespace Bot.Contracts.Calendar.Models
{
    public sealed class Day(DayOfWeek name, ushort number)
    {
        public DayOfWeek Name { get; set; } = name;
        public ushort Number { get; set; } = number;
    }
}
