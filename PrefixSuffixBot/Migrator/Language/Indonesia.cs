using System.Data.SQLite;
using System.Text.Json;

namespace PrefixSuffixBot.Migrator.Language;
public class Indonesia
{
    public Indonesia()
    {
        Generate();
    }

    private void Generate()
    {
        var rawText = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("../lang/id.json"))!;

        // Sending all text to database
        var dbName = "database.sqlite";
        var conString = $"Data Source={dbName};Version=3;";
        if (Directory.GetFiles("./").Where(x => x.Contains(dbName)).Count() == 0)
        {
            Console.WriteLine("Please generate database first!");
            return;
        }

        Console.WriteLine("Connecting to SQLite.");
        var con = new SQLiteConnection(conString);
        con.Open();

        foreach (var txt in rawText!)
        {
            Console.WriteLine($"Insert {txt.Key} to database.");
            var query = $"INSERT INTO keyword (keyword) VALUES (\"{txt.Key}\")";
            new SQLiteCommand(query, con).ExecuteNonQuery();
        }

        Console.WriteLine("Closing SQLite. Process done!");
        con.Close();
    }
}