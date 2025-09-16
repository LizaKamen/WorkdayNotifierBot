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
        var user = await UserRepository.GetUserForChatOrCreate(chatId, sender);

        switch (command)
        {
            case "/starttime":
                if (TimeOnly.TryParse(arg, out var time))
                {
                    user.StartDate = time;
                    await UserRepository.UpdateUser(user);
                    await botClient.SendMessage(chatId, $"{userName} updated start time to {time}", cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(chatId, "Please enter a valid time format.", cancellationToken: cancellationToken);
                }

                break;
            case "/utcoffset":
                if (int.TryParse(arg, out var offset))
                {
                    user.UtcOffset = offset;
                    await UserRepository.UpdateUser(user);
                    await botClient.SendMessage(chatId, $"{userName} updated utc offset to {offset}", cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(chatId, "Please enter a valid number.", cancellationToken: cancellationToken);
                }

                break;
            case "/status":
                await botClient.SendMessage(chatId,
                    $"{userName} stats: Start date: {user.StartDate}, Period: {user.Period}, Duration: {user.Duration}, UtcOffset: {user.UtcOffset}", cancellationToken: cancellationToken);
                break;
            case "/duration":
                if (int.TryParse(arg, out var duration))
                {
                    user.Duration = duration;
                    await UserRepository.UpdateUser(user);
                    await botClient.SendMessage(chatId, $"{userName} updated duration to {duration}", cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(chatId, "Please enter a valid time format.", cancellationToken: cancellationToken);
                }

                break;
            case "/period":
                if (int.TryParse(arg, out var period))
                {
                    user.Period = period;
                    await UserRepository.UpdateUser(user);
                    await botClient.SendMessage(chatId, $"{userName} updated period to {period}", cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(chatId, "Please enter a valid time format.", cancellationToken: cancellationToken);
                }

                break;
            case "/lastworkday":
                if (DateOnly.TryParse(arg, out var date))
                {
                    user.LastWorkDay = date;
                    await UserRepository.UpdateUser(user);
                    await botClient.SendMessage(chatId, $"{userName} updated last work date to {date}", cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(chatId, "Please enter a valid date format.", cancellationToken: cancellationToken);
                }

                break;
            case "/dayswowork":
                if (user.LastWorkDay != DateOnly.MinValue)
                {
                    var days = (DateTime.Today - user.LastWorkDay.ToDateTime(TimeOnly.MinValue)).Days;
                    await botClient.SendMessage(chatId, $"{userName} безработный уже {days} дней EZ", cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(chatId, $"{userName} не безработный, топ топ в офис", cancellationToken: cancellationToken);
                }

                break;
            case "/enable":
                await botClient.SendMessage(chatId, $"{userName} enabled notifications ", cancellationToken: cancellationToken);
                EnableNotificationsForChat(chatId, user, userName);
                break;
            case "/tillend":
                await botClient.SendMessage(chatId,
                    CalculateHoursAndCreateResponse(user, userName), cancellationToken: cancellationToken);
                break;
        }
    }

    private void EnableNotificationsForChat(string chatId, User user, string userName)
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
        var timeRemaining = user.StartDate.AddHours(user.Duration) -
                            TimeOnly.FromDateTime(DateTime.UtcNow).AddHours(user.UtcOffset);
        var hours = timeRemaining.Hours;
        var mins = timeRemaining.Minutes;
        var msg = hours > user.Duration
            ? $"{userName}, на сегодня все, отдыхай братик"
            : $"{userName}, До конца рабочего дня осталось {(hours > 0 ? $"{hours} часов" : "")} {(mins > 0 ? $"{mins} минут" : "")}";
        return msg;
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}