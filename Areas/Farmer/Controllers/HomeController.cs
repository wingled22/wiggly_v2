using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Areas.Farmer.Models;
using Wiggly.Entities;
using Wiggly.Identity;
using Wiggly.Models;

namespace Wiggly.Areas.Farmer.Controllers
{
    [Authorize(Roles = "Farmer")]
    [Area("Farmer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public HomeController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<HomeController> logger) 
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        // GET: HomeController
        
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;

                if (currentUser.IsInRole("Vendor"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Vendor" });
                }
            }
            return View();
        }

        public ActionResult GetSched(){
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var scheds = _context.Schedules.Where(q => q.Farmer == loggedInUser.Id).ToList();
            return Ok(scheds);
        }

        [HttpPost]
        public ActionResult SetSched(string values)
        {
            var loggedInUser =  _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var newAppointment = new FarmerAppointmentViewModel();
            JsonConvert.PopulateObject(values, newAppointment);

            if (!TryValidateModel(newAppointment))
                return BadRequest("error");

            var schedule = new Schedules() {
                Vendor = newAppointment.Vendor,
                BookingEndDate = newAppointment.BookingEndDate,
                BookingStartDate = newAppointment.BookingStartDate,
                Status = newAppointment.Status,
                Farmer = loggedInUser.Id
            };

            _context.Schedules.Add(schedule);
            _context.SaveChanges();
            return Ok();
        }


        public ActionResult GetVendors()
        {
            var res =  _context.AspNetUsers.Where(q => q.UserType == "Vendor").ToList();
            if (res == null)
                BadRequest("No Vendor ");
            return Ok(res);
        }
    }
}
