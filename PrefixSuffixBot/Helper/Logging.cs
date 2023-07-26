namespace PrefixSuffixBot.Helper;
public static class Logging
{
    public static DateTime Time = DateTime.Now;

    public static void Info(string message, string param = "INFO")
    {
        Console.WriteLine($"[{Time}/{param}] {message}");
        Console.ResetColor();
    }

    public static void Warning(string message, string param = "INFO")
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{Time}/{param}] [WARNING] {message}");
        Console.ResetColor();
    }

    public static void Error(Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{Time}] [ERROR] {exception.Message}");
        if (exception.Source != null)
            Console.WriteLine(exception.Source);
        if (exception.StackTrace != null)
            Console.WriteLine(exception.StackTrace);
        Console.ResetColor();
    }
}