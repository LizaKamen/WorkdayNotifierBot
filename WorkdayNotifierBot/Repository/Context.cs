using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorkdayNotifierBot;

public class Context : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public string DbPath { get; }
    
    public Context()
    {
        var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        DbPath = isDocker
            ? "/app/data/workdaynotifierbot.db"
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "workdaynotifierbot.db");
    }
    
    public Context(DbContextOptions<Context> options) : base(options)
    {
        DbPath = "design-time.db";
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .EnableSensitiveDataLogging()
                .UseSqlite($"Data Source={DbPath}");
        }
        
        base.OnConfiguring(optionsBuilder);
    }
}