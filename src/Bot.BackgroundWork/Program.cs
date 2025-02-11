using Bot.BackgroundWork;
using Bot.BackgroundWork.Services;
using Bot.Contracts.Repositories;
using Bot.Persistence;
using Bot.Persistence.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (_, services) =>
        {
            var connectionString = _.Configuration.GetConnectionString("PostgresDb");

            // Add persistence
            services.AddDbContext<RemindersDbContext>(options => options
                .UseNpgsql(connectionString));

            services.AddScoped<IReminderRepository, ReminderRepository>();

            // Add Hangfire
            services.AddHangfire(config => config
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

            services.AddHangfireServer(config =>
            {
                config.WorkerCount = 10;
                config.ServerName = "Reminder Bot Background Job Server";
            });

            // Add bot service.
            services.AddSingleton<ReminderBot>();

            // Add long polling service
            services.AddHostedService<Worker>();
        }
    )
    .Build();

using (var scope = host.Services.CreateScope())
{
    using var db = scope.ServiceProvider.GetRequiredService<RemindersDbContext>();
    db.Database.Migrate();
}

await host.RunAsync();
