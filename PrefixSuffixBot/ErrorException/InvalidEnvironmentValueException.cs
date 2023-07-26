namespace PrefixSuffixBot.ErrorException;
public class InvalidEnvironmentValueException : Exception
{
    public InvalidEnvironmentValueException(string envVal)
        : base($"Invalid value for {envVal} environment variable.") { }
}