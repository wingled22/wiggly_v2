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
    public class NotificationAPIController : Controller
    {
        private readonly ILogger<NotificationAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public NotificationAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<NotificationAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        public  IActionResult GetNotification()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var notif = _context.Notif.Where(q => q.Recipient == loggedInUser.Id).OrderBy(q => q.DateCreated).Take(20).ToList();

            return Ok(notif);
        }
    }
}
