using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Areas.Farmer.Controllers
{
    [Authorize(Roles = "Farmer")]
    [Area("Farmer")]
    public class ScheduleController : Controller
    {
        private readonly ILogger<ScheduleController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public ScheduleController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<ScheduleController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        [HttpGet()]
        public ActionResult GetSched(int? userId)
        {
            List<Schedules> scheds;

            if (userId == null || userId == 0)
            {
                var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
                scheds = _context.Schedules.Where(q => q.Farmer == loggedInUser.Id).ToList();
                return Ok(scheds);
            }
            else {
                scheds = _context.Schedules.Where(q => q.Vendor == userId).ToList();
                return Ok(scheds);
            }
        }

        [HttpGet]
        public ActionResult GetVendors()
        {
            var ret = new List<AspNetUsers>();
            var customAspNetuser = new AspNetUsers() {
                Id = 0,
                Firstname = "My Schedule",
                LastName = ""
            };
            ret.Add(customAspNetuser);
            var res = _context.AspNetUsers.Where(q => q.UserType == "Vendor").ToList();
            if (res == null)
                BadRequest("No Vendor ");

            ret.AddRange(res);
            return Ok(ret);
        }

        [HttpGet]
        public ActionResult GetUsers()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var ret = new List<AspNetUsers>();
            var res = _context.AspNetUsers.Where(q => q.Id != loggedInUser.Id && q.UserType != "Admin").ToList();
            if (res == null)
                BadRequest("No Vendor ");

            ret.AddRange(res);
            return Ok(ret);
        }

    }

}
