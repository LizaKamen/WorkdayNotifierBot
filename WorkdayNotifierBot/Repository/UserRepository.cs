using Telegram.Bot.Types;

namespace WorkdayNotifierBot.Repository;

public class UserRepository
{
    public static User GetUserForChatOrCreate(ChatId chatId, string userId)
    {
        using var context = new Context();
        var user = context.Users.FirstOrDefault(u => u.ChatId == chatId.ToString() && u.UserId == userId);
        if (user != null) return user;
        user = new User()
            { ChatId = chatId.ToString(), UserId = userId, StartDate = TimeOnly.Parse("8:00"), Duration = 9, Period = 30 };
        context.Users.Add(user);
        context.SaveChanges();
        return user;
    }

    public static void UpdateUser(User user)
    {
        using var context = new Context();
        var userToUpdate = GetUserForChatOrCreate(user.ChatId, user.UserId);
        context.Users.Update(userToUpdate);
        userToUpdate.Duration = user.Duration;
        userToUpdate.Period = user.Period;
        userToUpdate.StartDate = user.StartDate;
        userToUpdate.LastWorkDay = user.LastWorkDay;
        context.SaveChanges();
    }
}