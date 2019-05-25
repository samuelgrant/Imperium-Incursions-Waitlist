using Microsoft.Extensions.Logging;

/// <summary>
/// ILogger Extensions that allow us to make use of string.format syntax in our logs.
/// </summary>
public static class LoggerExtensions
{
    public static long GetEsiId(this string x)
    {
        return long.Parse(x.Split('/')[5]);
    }

    public static void LogDebugFormat(this ILogger log, string entry, params object[] args)
    {
        log.LogDebug(string.Format(entry, args));
    }

    public static void LogInformationFormat(this ILogger log, string entry, params object[] args)
    {
        log.LogInformation(string.Format(entry, args));
    }

    public static void LogWarningFormat(this ILogger log, string entry, params object[] args)
    {
        log.LogWarning(string.Format(entry, args));
    }

    public static void LogCriticalFormat(this ILogger log, string entry, params object[] args)
    {
        log.LogCritical(string.Format(entry, args));
    }

    public static void LogErrorFormat(this ILogger log, string entry, params object[] args)
    {
        log.LogError(string.Format(entry, args));
    }
}