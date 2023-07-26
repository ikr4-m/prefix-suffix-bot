using PrefixSuffixBot.Helper;

namespace PrefixSuffixBot;
public class PostEngine
{
    public PostEngine() { }

    public async void Spawn()
    {
        await Task.Delay(5000);
        Logging.Info("It works! (After 5s)", "POST");
    }
}