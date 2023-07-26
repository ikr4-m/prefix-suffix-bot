namespace PrefixSuffixBot.Helper;
public static class Logging
{
    public static void Info(string message, string param = "INFO")
    {
        Console.WriteLine($"[{DateTime.Now}/{param}] {message}");
        Console.ResetColor();
    }

    public static void Warning(string message, string param = "INFO")
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{DateTime.Now}/{param}] [WARNING] {message}");
        Console.ResetColor();
    }

    public static void Error(Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{DateTime.Now}] [ERROR] {exception.Message}");
        if (exception.Source != null)
            Console.WriteLine(exception.Source);
        if (exception.StackTrace != null)
            Console.WriteLine(exception.StackTrace);
        Console.ResetColor();
    }
}