using Microsoft.AspNetCore.Mvc;

namespace Imperium_Incursions_Waitlist.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Render()
        {
            int HttpCode = HttpContext.Response.StatusCode;

            // Forbidden
            if (HttpCode == 403)
                return View(viewName: "Forbidden");

            // Not Found
            if (HttpCode == 404)
                return View(viewName: "NotFound");

            // MethodNotAllowed
            if (HttpCode == 405)
                return View(viewName: "MethodNotAllowed");

            // Ban Page
            if (HttpCode == 418)
                return View(viewName: "Ban");

            // Oauth Failure 
            // State missmatch
            if (HttpCode == 452)
                return View(viewName: "OauthFailure");

            // Internal Server Error
            if (HttpCode == 500)
                return View(viewName: "InternalServerError");

            // Temporarily Unavailable || Maintenance Mode
            //if (HttpCode == 503)
            //    return View(viewName: "TemporarilyUnavailable");

            // Catch all remaining errors.
            return View(viewName: "GeneralError");
        }
    }
}