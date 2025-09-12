using Hangfire;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using WorkdayNotifierBot.Repository;

namespace WorkdayNotifierBot.Services;

public class TelegramBotHandler : IUpdateHandler
{
    private readonly string _botToken;
    private readonly IRecurringJobManager _recurringJobManager;

    public TelegramBotHandler(string botToken, IRecurringJobManager recurringJobManager)
    {
        _botToken = botToken;
        _recurringJobManager = recurringJobManager;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message is not { Text: { } messageText } message)
            return;

        var chatId = message.Chat.Id.ToString();

        var sender = message.From.Id.ToString();
        var userName = message.From.Username;
        var args = messageText.Split(' ');
        string command;
        var arg = string.Empty;
        command = args[0];
        if (args.Length > 1)
            arg = args[1];
        var user = UserRepository.GetUserForChatOrCreate(chatId, sender);

        switch (command)
        {
            case "/starttime":
                if (TimeOnly.TryParse(arg, out var time))
                {
                    user.StartDate = time;
                    UserRepository.UpdateUser(user);
                    await botClient.SendMessage(chatId, $"{userName} updated start time to {time}");
                }
                else
                {
                    await botClient.SendMessage(chatId, "Please enter a valid time format.");
                }

                break;
            case "/status":
                await botClient.SendMessage(chatId,
                    $"{userName} stats: Start date: {user.StartDate}, Period: {user.Period}, Duration: {user.Duration}.");
                break;
            case "/duration":
                if (int.TryParse(arg, out var duration))
                {
                    user.Duration = duration;
                    UserRepository.UpdateUser(user);
                    await botClient.SendMessage(chatId, $"{userName} updated duration to {duration}");
                }
                else
                {
                    await botClient.SendMessage(chatId, "Please enter a valid time format.");
                }

                break;
            case "/period":
                if (int.TryParse(arg, out var period))
                {
                    user.Period = period;
                    UserRepository.UpdateUser(user);
                    await botClient.SendMessage(chatId, $"{userName} updated period to {period}");
                }
                else
                {
                    await botClient.SendMessage(chatId, "Please enter a valid time format.");
                }

                break;
            case "/lastworkday":
                if (DateOnly.TryParse(arg, out var date))
                {
                    user.LastWorkDay = date;
                    UserRepository.UpdateUser(user);
                    await botClient.SendMessage(chatId, $"{userName} updated last work date to {date}");
                }
                else
                {
                    await botClient.SendMessage(chatId, "Please enter a valid date format.");
                }

                break;
            case "/dayswowork":
                if (user.LastWorkDay != DateOnly.MinValue)
                {
                    var days = (DateTime.Today - user.LastWorkDay.ToDateTime(TimeOnly.MinValue)).Days;
                    await botClient.SendMessage(chatId, $"{userName} безработный уже {days} дней EZ");
                }
                else
                {
                    await botClient.SendMessage(chatId, $"{userName} не безработный, топ топ в офис");
                }

                break;
            case "/enable":
                await botClient.SendMessage(chatId, $"{userName} enabled notifications ");
                await EnableNotificationsForChat(chatId, user, userName);
                break;
            case "/tillend":
                await botClient.SendMessage(chatId,
                    CalculateHoursAndCreateResponse(user, userName));
                break;
        }
    }

    private async Task EnableNotificationsForChat(string chatId, User user, string userName)
    {
        var jobId = $"tg-notification-{chatId}";
        _recurringJobManager.RemoveIfExists(jobId);
        _recurringJobManager.AddOrUpdate(
            jobId,
            () => SendMessage(_botToken, chatId, user, userName),
            $"*/{user.Period} * * * *");
    }

    public static async Task SendMessage(string token, string chatId, User user, string userName)
    {
        var botClient = new TelegramBotClient(token);
        await botClient.SendMessage(chatId, CalculateHoursAndCreateResponse(user, userName));
    }

    public static string CalculateHoursAndCreateResponse(User user, string userName)
    {
        var timeRemaining = user.StartDate.AddHours(user.Duration) - TimeOnly.FromDateTime(DateTime.Now);
        var hours = timeRemaining.Hours;
        var mins = timeRemaining.Minutes;
        var msg = hours > user.Duration
            ? $"{userName}, на сегодня все, отдыхай братик"
            : $"{userName}, До конца рабочего дня осталось {hours} часов {mins} минут";
        return msg;
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        throw exception;
    }
}