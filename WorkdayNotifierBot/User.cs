using Telegram.Bot.Types;

namespace WorkdayNotifierBot;

public class User
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public string ChatId { get; set; }
    public TimeOnly StartDate { get; set; }
    public int Duration { get; set; }
    public int Period { get; set; }
    public DateOnly LastWorkDay  { get; set; }
}