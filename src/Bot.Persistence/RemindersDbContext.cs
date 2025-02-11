using Bot.Contracts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bot.Persistence
{
    public class RemindersDbContext(DbContextOptions<RemindersDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ToSnakeCaseTables();
        }

        public DbSet<Reminder> Reminders { get; set; }
    }
}
