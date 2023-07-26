using PrefixSuffixBot.Helper;

namespace PrefixSuffixBot;
public class PostEngine
{
    private DatabaseContext _db;

    public PostEngine(DatabaseContext db)
    {
        _db = db;
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
            Environment.Exit(1);
        }
    }

    public async void Spawn()
    {
        await Task.Delay(5000);
        Logging.Info("It works! (After 5s)", "POST");
    }
}