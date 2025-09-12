using Hangfire;
using Hangfire.SQLite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using WorkdayNotifierBot.Services;

namespace WorkdayNotifierBot;

class Program
{
    static async Task Main(string[] args)
    {
        using var dbContext = new Context();
        dbContext.Database.Migrate();

        var token = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build()["tokens:botToken"];

        var sqliteOptions = new SQLiteStorageOptions
        {
            QueuePollInterval = TimeSpan.FromSeconds(15),
            InvisibilityTimeout = TimeSpan.FromHours(1),
        };

        const string dbPath = "hangfire.db";

        if (File.Exists(dbPath))
            File.Delete(dbPath);

        GlobalConfiguration.Configuration
            .UseSQLiteStorage($"Data Source={dbPath};", sqliteOptions)
            .UseColouredConsoleLogProvider();

        var recurringJobManager = new RecurringJobManager();

        var botHandler = new TelegramBotHandler(token, recurringJobManager);

        var botClient = new TelegramBotClient(token);

        using (var server = new BackgroundJobServer(
                   new BackgroundJobServerOptions
                   {
                       WorkerCount = 1
                   }))
        {
            _ = botClient.ReceiveAsync(botHandler);
            Console.ReadLine();
        }
    }
}