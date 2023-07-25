using PrefixSuffixBot.Migrator;

namespace PrefixSuffixBot;
public class Program
{
    public static void Main(string[] args)
    {
        // Check if args want to migrate
        if (args.Contains("generate"))
        {
            new MigratorEngine(args).Start();
            return;
        }

        Console.WriteLine("Here's belong some code for looping");
    }
}