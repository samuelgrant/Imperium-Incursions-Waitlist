using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

/// <summary>
/// ILogger Extensions that allow us to make use of string.format syntax in our logs.
/// </summary>
public static class LoggerExtensions
{
    public static long GetEsiId(this string x) => long.Parse(x.Split('/')[5]);

    public static int AccountId(this ClaimsPrincipal user) => int.Parse(user.FindFirstValue("Id"));

    public static int PreferredPilotId(this IRequestCookieCollection cookies) => int.Parse(cookies["prefPilot"].Split(':')[0]);

    public static string PreferredPilotName(this IRequestCookieCollection cookies) => cookies["prefPilot"].Split(':')[1];

    // Two loggers cannot access LogWarning for some reason so I've put this here.
    public static void LogWarning(this ILogger log, string entry, params object[] args) => log.LogWarning(string.Format(entry, args));
}