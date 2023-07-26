using PrefixSuffixBot.Helper;
using System.Data.SQLite;
using System.Text.Json;

namespace PrefixSuffixBot.Migrator;
public class MigratorEngine
{
    private string[] _args;
    private string _dir = AppDomain.CurrentDomain.BaseDirectory;

    public MigratorEngine(string[] args)
    {
        _args = args.Skip(1).ToArray();
    }

    public void Start()
    {
        if (_args.Contains("db"))
        {
            GenerateDatabase();
            return;
        }
        else if (_args.Contains("lang"))
        {
            if (_args.Count() == 1)
            {
                Console.Write("Usage: [db | lang <code>]");
                return;
            }
            ImportLanguage(_args[1]);
            return;
        }

        Console.Write("Usage: [db | lang <code>]");
    }

    private void GenerateDatabase()
    {
        // List all SQL file
        var inDirFiles = Directory.GetFiles(Path.Join(_dir, "./Migrator/SQL"));
        
        // Create SQLite database
        var dbName = Path.Join(_dir, "database.sqlite");
        var conString = $"Data Source={dbName};Version=3;";
        if (Directory.GetFiles(_dir).Where(x => x.Contains(dbName)).Count() == 0)
        {
            SQLiteConnection.CreateFile(dbName);
            Console.WriteLine("Creating SQLite.");
        }

        Logging.Info("Connecting to SQLite.");
        var con = new SQLiteConnection(conString);
        con.Open();
        
        // Import all SQL Database
        foreach (var path in inDirFiles)
        {
            var text = File.ReadAllText(path);
            Logging.Info($"Import SQL file from {path}");
            Console.WriteLine(text);
            new SQLiteCommand(text, con).ExecuteNonQuery();
        }

        Logging.Info("Closing SQLite. Process done!");
        con.Close();
    }

    private void ImportLanguage(string lang)
    {
        var jsonPath = Path.Join(_dir, $"./Migrator/lang/{lang}.json");
        if (!Path.Exists(jsonPath))
        {
            Logging.Error(new Exception("Language not supported."));
            Environment.Exit(1);
        }
        var rawText = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(jsonPath))!;

        // Sending all text to database
        var dbPath = Path.Join(_dir, "database.sqlite");
        if (!Path.Exists(dbPath))
        {
            Logging.Error(new Exception("Database not found. Please generate first."));
            Environment.Exit(1);
        }

        Logging.Info("Connecting to SQLite.");
        var conString = $"Data Source={dbPath};Version=3;";
        var con = new SQLiteConnection(conString);
        con.Open();

        foreach (var txt in rawText!)
        {
            Logging.Info($"Insert {txt.Key} to database.");
            var query = $"INSERT INTO keyword (keyword) VALUES (\"{txt.Key}\")";
            new SQLiteCommand(query, con).ExecuteNonQuery();
        }

        Logging.Info("Closing SQLite. Process done!");
        con.Close();
    }
}