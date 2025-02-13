using Bot.Contracts.Calendar.Models;
using IKB = Telegram.BotAPI.AvailableTypes.InlineKeyboardButton;

namespace Bot.BackgroundWork.Calendar
{
    public static class CalendarInput
    {
        public static IKB[][] New(DateTime dateTime)
        {
            var year = dateTime.Year;
            var keyboard = new IKB[6][];
            keyboard[0] = [new IKB($"{year}") { CallbackData = $"Год {year}" }];
            for (int i = 1, n = 0; i < 5; i++)
            {
                keyboard[i] = new IKB[3];
                for (int j = 0; j < 3; j++, n++)
                {
                    var month = (MonthName)n;
                    keyboard[i][j] = new IKB($"{month}") { CallbackData = $"месяц {year} {n}" };
                }
            }
            keyboard[5] =
            [
                new IKB($"{year - 1}") { CallbackData = $"год {year - 1}" },
            new IKB($"{year + 1}") { CallbackData = $"год {year + 1}" },
        ];
            return keyboard;
        }

        public static IKB[][] New(Month mon)
        {
            var calendar = new IKB[mon.Weeks + 3][];
            var pos = 0;
            calendar[0] = [new IKB($"{mon.Name} {mon.Year}") { CallbackData = $"год {mon.Year}" }];
            var days = new[] { "Вс", "Пн", "Вт", "Ср", "Чт", "Пт", "Сб" };
            calendar[1] = new IKB[7];
            for (int i = 0; i < 7; i++)
            {
                calendar[1][i] = new IKB(days[i]) { CallbackData = $"{((DayOfWeek)i)}" };
            }
            for (int i = 2; i < mon.Weeks + 2; i++)
            {
                calendar[i] = new IKB[7];
                for (int j = 0; j < 7; j++)
                {
                    if (pos < mon.Days.Length)
                    {
                        if ((int)mon.Days[pos].Name == j)
                        {
                            string monthNumString;
                            int monthNum = (int)mon.Name + 1;
                            if (monthNum < 10)
                            {
                                monthNumString = $"0{monthNum}";
                            }
                            else
                            {
                                monthNumString = $"{monthNum}";
                            }
                            calendar[i][j] = new IKB($"{mon.Days[pos].Number}")
                            {
                                CallbackData = $"{mon.Days[pos].Number}.{monthNumString}.{mon.Year}"
                            };
                            pos++;
                        }
                        else
                        {
                            calendar[i][j] = new IKB("*") { CallbackData = "Пусто" };
                        }
                    }
                    else
                    {
                        calendar[i][j] = new IKB("*") { CallbackData = "Пусто" };
                    }
                }
            }
            calendar[^1] = new IKB[2];
            var previousMonth = mon.Name == MonthName.Январь ? MonthName.Декабрь : mon.Name - 1;
            var nextMonth = mon.Name == MonthName.Декабрь ? MonthName.Январь : mon.Name + 1;
            var previousYear = previousMonth == MonthName.Декабрь ? mon.Year - 1 : mon.Year;
            var nextYear = nextMonth == MonthName.Январь ? mon.Year + 1 : mon.Year;
            calendar[^1][0] = new IKB($"{previousMonth}")
            {
                CallbackData = $"месяц {previousYear} {(ushort)previousMonth}",
            };
            calendar[^1][1] = new IKB($"{nextMonth}")
            {
                CallbackData = $"месяц {nextYear} {(ushort)nextMonth}",
            };
            return calendar;
        }

        public static IKB[][] New(uint year)
        {
            var keyboard = new IKB[6][];
            keyboard[0] = [new IKB($"{year}") { CallbackData = $"Год {year}" }];
            for (int i = 1, n = 0; i < 5; i++)
            {
                keyboard[i] = new IKB[3];
                for (int j = 0; j < 3; j++, n++)
                {
                    var month = (MonthName)n;
                    keyboard[i][j] = new IKB($"{month}") { CallbackData = $"месяц {year} {n}" };
                }
            }
            keyboard[5] =
            [
                new IKB($"{year - 1}") { CallbackData = $"год {year - 1}" },
            new IKB($"{year + 1}") { CallbackData = $"год {year + 1}" },
        ];
            return keyboard;
        }
    }
}
