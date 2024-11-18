namespace DataAccess;

public class DatabaseBackup : IDatabaseBackup
{
    private const string DatabasePath = "Ancestry.db";
    private const string BackupFolderPath = "Backups";

    public void BackupDatabase()
    {
        Directory.CreateDirectory(BackupFolderPath);

        if (!File.Exists(DatabasePath))
        {
            Console.WriteLine("Database file not found.");
            return;
        }

        var backupFileName = Path.Combine(BackupFolderPath, $"Ancestry_backup_{DateTime.UtcNow:yyyyMMddHHmmss}.db");

        File.Copy(DatabasePath, backupFileName);

        Console.WriteLine($"Database backed up to {backupFileName}");

        var backupFiles = new DirectoryInfo(BackupFolderPath)
            .GetFiles("Ancestry_backup_*.db")
            .OrderBy(f => f.CreationTimeUtc)
            .ToArray();

        if (backupFiles.Length > 100)
        {
            backupFiles.First().Delete();
            Console.WriteLine($"Deleted backup file {backupFiles.First().FullName}");
        }
    }
}