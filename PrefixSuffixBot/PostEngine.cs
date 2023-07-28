using PrefixSuffixBot.Helper;

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

    public async void Spawn()
    {
        await Task.Delay(5000);
        Logging.Info("It works! (After 5s)", "POST");
    }
}