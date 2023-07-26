using PrefixSuffixBot.Helper;
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
        var db = new DatabaseContext();
        if (File.Exists(db.Directory)) File.Delete(db.Directory);
        db.Database.EnsureCreated();
        Logging.Info("Generate/regenerate database from model successfully!");
    }

    private void ImportLanguage(string lang)
    {
        var db = new DatabaseContext();

        var jsonPath = Path.Join(_dir, $"./Migrator/lang/{lang}.json");
        if (!Path.Exists(jsonPath))
        {
            Logging.Error(new Exception("Language not supported."));
            Environment.Exit(1);
        }
        var rawText = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(jsonPath))!;

        foreach (var txt in rawText)
            db.Keywords.Add(new Keyword { KeywordText = txt.Key });

        
        Logging.Info($"Importing text file....");
        db.SaveChanges();
        Logging.Info($"Successfully import {rawText.Count} keyword");

        var count = db.Keywords.Count();
        Logging.Info($"Total keyword: {count}");
    }
}