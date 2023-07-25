using System.IO;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace PrefixSuffixBot.Migrator;
public class MigratorEngine
{
    private string[] _args;

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
            return;
        }

        Console.Write("Usage: [db | lang <code>]");
    }

    private void GenerateDatabase()
    {
        // List all SQL file
        var inDirFiles = Directory.GetFiles("./Migrator/SQL");
        
        // Create SQLite database
        var dbName = "database.sqlite";
        var conString = $"Data Source={dbName};Version=3;";
        if (Directory.GetFiles("./").Where(x => x.Contains(dbName)).Count() == 0)
        {
            SQLiteConnection.CreateFile(dbName);
            Console.WriteLine("Creating SQLite.");
        }

        Console.WriteLine("Connecting to SQLite.");
        var con = new SQLiteConnection(conString);
        con.Open();
        
        // Import all SQL Database
        foreach (var path in inDirFiles)
        {
            var text = File.ReadAllText(path);
            Console.WriteLine($"Import SQL file from {path}");
            Console.WriteLine(text);
            new SQLiteCommand(text, con).ExecuteNonQuery();
        }

        Console.WriteLine("Closing SQLite. Process done!");
        con.Close();
    }

    private void MigrateDown(string timestamp)
    {
        //
    }
}