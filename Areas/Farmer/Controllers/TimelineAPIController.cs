using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Areas.Farmer.Models;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Areas.Farmer.Controllers
{
    [Authorize(Roles = "Farmer")]
    [Area("Farmer")]
    public class TimelineAPIController : Controller
    {
        private readonly ILogger<TimelineAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public TimelineAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<TimelineAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        public IActionResult GetFarmerTimelineData()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var now = DateTime.Now.AddDays(3);
            var end = DateTime.Now.AddMinutes(60);

            //TODO: display timeline data 2 days before the booking
            //var timelinedata = _context.Schedules.Where(q => 
            //                        q.Farmer == loggedInUser.Id &&
            //                        //q.BookingStartDate <= DateTime.Now && 
            //                        q.BookingStartDate > now ).Where(q=>q.BookingStartDate < DateTime.Now)
            //    .Select(q => new TimelineViewModel { 
            //        Date = ((DateTime)q.BookingStartDate).ToString("MMMM dd, yyyy H:mm:ss"),
            //        Agenda = q.Notes })
            //    .ToList();

            var timelinedata = (from sched in _context.Schedules
                                where sched.Farmer == loggedInUser.Id && sched.BookingStartDate >= DateTime.Now
                                   && sched.BookingStartDate <= now
                                //&& sched.BookingEndDate <= DateTime.Now
                                select new TimelineViewModel
                                {
                                    Date = ((DateTime)sched.BookingStartDate).ToString("MMMM dd, yyyy H:mm:ss"),
                                    Agenda = sched.Notes
                                }).ToList();
                                

            return Ok(timelinedata);
        }
    }
}
