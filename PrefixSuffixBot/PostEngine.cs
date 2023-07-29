using PrefixSuffixBot.Helper;
using Microsoft.EntityFrameworkCore;

namespace PrefixSuffixBot;
public class PostEngine
{
    private DatabaseContext _db;
    private MastodonOAuth _mastodon;

    public PostEngine(DatabaseContext db, MastodonOAuth mastodon)
    {
        _db = db;
        _mastodon = mastodon;
    }

    public void CheckConnection()
    {
        try
        {
            Logging.Info($"Preparing {_db.Keywords.Count()} keywords.");
        }
        catch (System.Exception e)
        {
            Logging.Error(e);
            Logging.Info("Maybe you didn't generate the db?");
            Environment.Exit(1);
        }
    }

    public async Task Spawn()
    {
        // await Task.Delay(5000);
        // Logging.Info("It works! (After 5s)", "POST");

        var data = await _db.Keywords.Where(x => !x.IsUsed).ToListAsync();
        if (data == null)
        {
            Logging.Warning("Keywords is empty or it being used all. Ignore.", "POST");
            return;
        }

        var prefix = Environment.GetEnvironmentVariable("PREFIX_POST");
        var suffix = Environment.GetEnvironmentVariable("SUFFIX_POST");
        if (prefix == null && suffix == null)
        {
            Logging.Warning("Neither prefix or suffix is empty, please check your env. Ignore.", "POST");
            return;
        }
        prefix = prefix == null ? "" : $"{prefix} ";
        suffix = suffix == null ? "" : $" {suffix}";

        var randNumber = new Random().Next(data.Count) - 1;
        var pickedKeyword = data[randNumber];
        var strKeyword = prefix + pickedKeyword.KeywordText + suffix;
        Logging.Info($"Picking new word, {pickedKeyword.KeywordText}. Publishing to instance.", "POST");
        await _mastodon.PostToMastodon(strKeyword, pickedKeyword);
    }
}