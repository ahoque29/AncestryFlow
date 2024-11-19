using DataAccess;

namespace TestConsole;

public static class Program
{
    public static async Task Main()
    {
        var databaseBackup = new DatabaseBackup();
        databaseBackup.BackupDatabase();
        await using var context = new DatabaseContext();
        await context.InitializeDatabase();
    }
}