using PrefixSuffixBot.ErrorException;
using PrefixSuffixBot.Helper;
using PrefixSuffixBot.Migrator;

namespace PrefixSuffixBot;
public class Program
{
    private int _loopInMinute = 0;

    public void Pool()
    {
        // Checking environment for loop, default is 5 minute
        var loopEnv = Environment.GetEnvironmentVariable("LOOP_LENGTH") ?? "5";
        if (!loopEnv.All(char.IsDigit))
        {
            Logging.Error(new InvalidEnvironmentValueException("LOOP_LENGTH"));
            Environment.Exit(1);
        }
        _loopInMinute = int.Parse(loopEnv);

        // TODO: Make a loop that respecting "server timedate"
        // Like, when this is 08:21 PM and it need to loop at 5 min every time
        // it will start looping on 08:25 without waiting the another thread completely
        // finish.
        while (true)
        {
            // Spawning Task Thread
            // Calculate next turn
            var dateNow = DateTime.Now;
            var diffMin = _loopInMinute - (dateNow.Minute % _loopInMinute);
            var nextTurnDT = dateNow.AddMinutes(diffMin);
            var nextTurn = (int) double.Floor(nextTurnDT.Subtract(dateNow).TotalMilliseconds);

            Logging.Info(
                $"Diff min is {diffMin} from local server. Your next turn is on {nextTurnDT}. [{nextTurn} ms]");
            Thread.Sleep(nextTurn);
        }
    }

    public static void Main(string[] args)
    {
        // Check if args want to migrate
        if (args.Contains("generate"))
        {
            new MigratorEngine(args).Start();
            return;
        }

        Logging.Info("Starting Pool", "INIT");
        new Thread(new Program().Pool).Start();
    }
}