using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imperium_Incursions_Waitlist.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
    [Route("/api/v1/announcements")]
    public class AnnouncmentController : Controller
    {
        private readonly Data.WaitlistDataContext _Db;
        private readonly ILogger _Logger;

        public AnnouncmentController(Data.WaitlistDataContext db, ILogger<AnnouncmentController> logger)
        {
            _Db = db;
            _Logger = logger;
        }

        [HttpGet("")]
        [Produces("application/json")]
        public IActionResult Index()
        {
            var data = _Db.Announcements.Where(c => c.DeletedAt == null).Select(s => new {
                    s.Id,
                    s.Message,
                    s.Type,
                    posted = s.TimeDiff(),
                    s.CreatedAt,
                    createdBy = s.CreatorAdmin.Name
                }).OrderBy(o => o.CreatedAt)                
                .Take(1).FirstOrDefault();

            // Allows the User to hide an alert
            if(Request.Cookies["hide_banner"] != null)
            {
                int.TryParse(Request.Cookies["hide_banner"].ToString(), out int IgnoreId);
                if (data != null && data.Id == IgnoreId)
                    return Accepted(data);
            }

            return Ok(data);
        }

        [HttpPost("")]
        [Produces("application/json")]
        [Authorize(Roles = "Commander,Leadership,Dev")]
        public IActionResult SaveAnnouncment(IFormCollection request)
        {
            try
            {
                Announcement announcement = new Announcement
                {
                    CreatorAdminId = User.AccountId(),
                    Type = request["type"].ToString(),
                    Message = request["message"].ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                _Db.Add(announcement);
                _Db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error creating an announcment. The admin was {0}: {1}", User.AccountName(), ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id:int}/hide")]
        [Produces("application/json")]
        public IActionResult Hide(int id)
        {
            CookieOptions options = new CookieOptions
            {
                IsEssential = true,
                Expires = DateTime.Now.AddMinutes(5)
            };


            if (Request.Cookies["hide_banner"] == null)
                Response.Cookies.Delete("hide_banner");

            Response.Cookies.Append("hide_banner", id.ToString(), options);

            return Ok();
        }



        [HttpDelete("{id}")]
        [Produces("application/json")]
        [Authorize(Roles = "Commander,Leadership,Dev")]
        public IActionResult DeleteAnnouncment(int id)
        {
            Announcement announcement = _Db.Announcements.Find(id);
            announcement.DeletedAt = DateTime.UtcNow;
            _Db.SaveChanges();

            return Ok();
        }
    }
}