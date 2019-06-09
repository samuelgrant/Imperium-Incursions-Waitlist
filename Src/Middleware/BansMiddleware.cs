using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Imperium_Incursions_Waitlist.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Authentication;

public class BansMiddleware
{
    private readonly RequestDelegate _next;
    private WaitlistDataContext _Db;

    public BansMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, WaitlistDataContext db)
    {
        _Db = db;

        //If user is not authenticated
        if(!context.User.Identity.IsAuthenticated)
        {
            await _next.Invoke(context);
        }
        else
        {
            var account = _Db.Accounts.Include(c => c.AccountBans).FirstOrDefault( 
                    c => c.Id == context.User.AccountId()
                );

            if(account == null || !account.IsBanned())
            {
                await _next.Invoke(context);
            }
            else
            {//context.Request.Headers["X-Requested-With"] >>> {XMLHttpRequest}
                if(context.Request.Headers["X-Requested-With"].ToString() == "XMLHttpRequest")
                {
                    context.Response.StatusCode = 404;
                    await _next.Invoke(context);
                }
                    

                await context.SignOutAsync();
                context.Response.Redirect("/error/banned");
            }
        }
    }
}

public static class GiceBansUpdateMiddlewareExtensions
{
    public static IApplicationBuilder UseBans(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<BansMiddleware>();
    }
}