﻿namespace Bot.Contracts.Calendar.Models
{
    public sealed class Month
    {
        public Month(MonthName monthName, uint year)
        {
            this.Name = monthName;
            this.Year = year;
            bool leapYear = this.Year % 4 == 0;
            int days = this.Name switch
            {
                MonthName.Февраль => leapYear ? 29 : 28,
                MonthName.Апрель => 30,
                MonthName.Июнь => 30,
                MonthName.Сентябрь => 30,
                MonthName.Ноябрь => 30,
                _ => 31,
            };
            this.Days = new Day[days];
            long firstDay =
                (year * 365)
                + (leapYear ? -1 : 0)
                + ((year - (year % 4)) / 4)
                - ((year - (year % 400)) / 400)
                + 4;
            int month = (int)monthName;
            firstDay += month < 1 ? 0 : 31;
            firstDay +=
                month < 2 ? 0
                : leapYear ? 29
                : 28;
            firstDay += month < 3 ? 0 : 31;
            firstDay += month < 4 ? 0 : 30;
            firstDay += month < 5 ? 0 : 31;
            firstDay += month < 6 ? 0 : 30;
            firstDay += month < 7 ? 0 : 31;
            firstDay += month < 8 ? 0 : 31;
            firstDay += month < 9 ? 0 : 30;
            firstDay += month < 10 ? 0 : 31;
            firstDay += month < 11 ? 0 : 30;
            firstDay %= 7;
            for (int i = 0; i < this.Days.Length; i++)
            {
                this.Days[i] = new Day((DayOfWeek)((i + firstDay) % 7), (ushort)(i + 1));
            }
        }

        public uint Year { get; set; }
        public MonthName Name { get; set; }
        public Day[] Days { get; set; }

        public ushort Weeks
        {
            get
            {
                int days = (int)this.Days[0].Name + this.Days.Length - 1;
                return (ushort)(((days - (days % 7)) / 7) + (days % 7 > 0 ? 1 : 0));
            }
        }
    }
}
