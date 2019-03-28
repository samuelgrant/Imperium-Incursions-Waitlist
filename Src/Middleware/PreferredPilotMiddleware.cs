using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

public class PreferredPilotMiddleware
{
    private readonly RequestDelegate _next;

    public PreferredPilotMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string requestController = context.GetRouteData().Values["controller"].ToString();

        
        // The following controllers bypass this middleware as they need to be able to work in order to allow a pilot to be selected
        // 1) PilotSelectController, 2) EveController, 3) GiceController, as well as 4) Unauthenticated users
        if (context.User.FindFirst("id") == null || requestController == "PilotSelect" || requestController == "Eve" || requestController == "Gice")
            await _next.Invoke(context);

        // If a preferred pilot is not selected 
        // redirect to the SelectPilot.cshtml view.
        if (context.Request.Cookies["prefPilot"] == null)
            context.Response.Redirect("/pilot-select");

        await _next.Invoke(context);
    }
}

public static class PreferredPilotMiddlewareExtensions
{
    public static IApplicationBuilder UsePreferredPilotMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PreferredPilotMiddleware>();
    }
}