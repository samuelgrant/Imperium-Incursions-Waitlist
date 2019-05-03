using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Imperium_Incursions_Waitlist.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Imperium_Incursions_Waitlist.Models;


public class RoleSessionUpdateMiddleware
{
    private readonly RequestDelegate _next;
    private WaitlistDataContext _Db;

    public RoleSessionUpdateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, WaitlistDataContext db)
    {
        _Db = db;

        //If user is not authenticated
        if (!context.User.Identity.IsAuthenticated)
        {
            await _next.Invoke(context);
        }
        else
        {
            var user = context.User as ClaimsPrincipal;
            var identity = user.Identity as ClaimsIdentity;

            var claims = user.Claims.Where(c => c.Type == ClaimTypes.Role).ToArray();

            for (int i = 0; i < claims.Length; i++)
            {
                identity.RemoveClaim(claims[i]);
            }

            var roles = _Db.Accounts.Include(a => a.AccountRoles).ThenInclude(ar => ar.Role)
                .Where(a => a.Id == int.Parse(user.FindFirst("id").Value)).SingleOrDefault();

            if (roles != null)
            {
                foreach (var role in roles?.AccountRoles)
                    identity.AddClaim(new Claim(ClaimTypes.Role, role.Role.Name));
            }


            await _next.Invoke(context);
        }
    }
}

public static class RoleSessionUpdateMiddlewareExtensions
{
    public static IApplicationBuilder UseSessionBasedRoles(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RoleSessionUpdateMiddleware>();
    }
}