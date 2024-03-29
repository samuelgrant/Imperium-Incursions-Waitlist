﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Imperium_Incursions_Waitlist.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private Data.WaitlistDataContext _Db;

        public HomeController(Data.WaitlistDataContext db) => _Db = db;

        [Route("/search")]
        [Produces("application/json")]
        public IActionResult Search(string q, string filter = "")
        {
            List<string> results = new List<string>();

            if(filter.ToLower() != "account")
            {
                var pilots = from p in _Db.Pilots select p;
                if (!String.IsNullOrEmpty(q))
                {
                    pilots = pilots.Where(s => s.CharacterName.Contains(q));
                }

                foreach (var p in pilots)
                    if (p.CharacterName.ToLower() != "system")
                        results.Add(p.CharacterName);
            }

            if(filter.ToLower() != "pilot")
            {
                var accounts = from a in _Db.Accounts select a;
                if (!String.IsNullOrEmpty(q))
                {
                    accounts = accounts.Where(s => s.Name.Contains(q));

                    foreach (var a in accounts)
                        if(a.Name.ToLower() != "system")
                            results.Add(a.Name);
                }
            }

            return Ok(results);
        }
    }
}