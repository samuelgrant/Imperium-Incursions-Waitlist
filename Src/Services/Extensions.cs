using Imperium_Incursions_Waitlist;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;

/// <summary>
/// ILogger Extensions that allow us to make use of string.format syntax in our logs.
/// </summary>
public static class LoggerExtensions
{
    public static long GetEsiId(this string x) => long.Parse(x.Split('/')[5]);

    public static int AccountId(this ClaimsPrincipal user) => int.Parse(user.FindFirstValue("Id"));

    public static string AccountName(this ClaimsPrincipal user) => user.FindFirstValue("name");

    public static int PreferredPilotId(this IRequestCookieCollection cookies) => int.Parse(cookies["prefPilot"].Split(':')[0]);

    public static string PreferredPilotName(this IRequestCookieCollection cookies) => cookies["prefPilot"].Split(':')[1];

    // Two loggers cannot access LogWarning for some reason so I've put this here.
    public static void LogWarning(this ILogger log, string entry, params object[] args) => log.LogWarning(string.Format(entry, args));

    // Returns an enum representation of why the fleet members api errored
    public static FleetErrorType? ErrorType(this ESI.NET.EsiResponse<List<ESI.NET.Models.Fleets.Member>> x)
    {
        if (x.Message.Contains("The specified proxy or server node") && x.Message.Contains("is dead"))
            return FleetErrorType.FleetDead;

        if (x.Message.Contains("The fleet does not exist or you don't have access to it"))
            return FleetErrorType.InvalidBoss;

        return null;
    }

    // Returns an enum representation of why the fleet members api errored
    public static FleetErrorType? ErrorType(this ESI.NET.EsiResponse<List<ESI.NET.Models.Fleets.Wing>> x)
    {
        if (x.Message.Contains("The specified proxy or server node") && x.Message.Contains("is dead"))
            return FleetErrorType.FleetDead;

        if (x.Message.Contains("The fleet does not exist or you don't have access to it"))
            return FleetErrorType.InvalidBoss;

        return null;
    }

    public static string _str(this IFormCollection request, string key)
    {
        return request[key].ToString();
    }

    public static int _int(this IFormCollection request, string key)
    {
        int.TryParse(request[key].ToString(), out int x);

        return x;
    }
}