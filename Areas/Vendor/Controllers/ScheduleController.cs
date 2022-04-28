using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wiggly.Areas.Farmer.Models;
using Wiggly.Areas.Vendor.Models;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Areas.Vendor.Controllers
{

    //[Authorize(Roles = "Vendor")]
    [Area("Vendor")]
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

        public ActionResult GetSched()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var scheds = _context.Schedules.Where(q => q.Vendor == loggedInUser.Id).ToList();
            return Ok(scheds);
        }

        [HttpPost]
        public ActionResult SetSched(string values)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var newAppointment = new VendorAppointmentViewModel();
            JsonConvert.PopulateObject(values, newAppointment);

            if (!TryValidateModel(newAppointment))
                return BadRequest("error");

            var schedule = new Schedules()
            {
                Farmer = newAppointment.Farmer,
                BookingEndDate = newAppointment.BookingEndDate,
                BookingStartDate = newAppointment.BookingStartDate,
                Status = newAppointment.Status,
                Vendor = loggedInUser.Id,
                Notes = newAppointment.Notes,
                DateCreated = DateTime.Now
            };

            _context.Schedules.Add(schedule);
            _context.SaveChanges();
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> UpdateSched(int? key, string values)
        {
            if (key == null)
            {
                return BadRequest("Id has no value");
            }

            var sched = _context.Schedules.Where(q => q.SchedId == key).FirstOrDefault();

            if (sched == null)
            {
                return BadRequest("Sched not found");
            }


            JsonConvert.PopulateObject(values, sched);
            //book.AvailableCopies = book.NoCopies;

            if (!TryValidateModel(sched))
                return BadRequest("values are incorrect");

            _context.Schedules.Update(sched);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSched(int key)
        {

            var toDelete = await _context.Schedules.FindAsync(key);
            if (toDelete == null)
            {
                return BadRequest("Item is already deleted.(Or doesn't exist on the system)");
            }

            _context.Schedules.Remove(toDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet]
        public ActionResult GetUsers()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var ret = new List<AspNetUsers>();
            var res = _context.AspNetUsers.Where(q => q.Id != loggedInUser.Id).ToList();
            if (res == null)
                BadRequest("No Vendor ");

            ret.AddRange(res);
            return Ok(ret);
        }
    }
}
