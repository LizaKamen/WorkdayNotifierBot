using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WorkdayNotifierBot.Repository;

namespace WorkdayNotifierBot;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {
        // Для времени дизайна используем фиксированный путь
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "workdaynotifierbot.db");
            
        var optionsBuilder = new DbContextOptionsBuilder<Context>();
        optionsBuilder
            .UseSqlite($"Data Source={dbPath}")
            .EnableSensitiveDataLogging();
            
        return new Context(optionsBuilder.Options);
    }
}