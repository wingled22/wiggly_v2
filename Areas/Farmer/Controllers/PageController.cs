using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class PageController : Controller
    {

        private readonly ILogger<PageController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public PageController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<PageController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View("Index-v2");
        }

        public IActionResult Calendar()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }

        public IActionResult Transaction()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }

        public IActionResult Chat()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }


        public IActionResult Marketplace()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }

        public IActionResult Profile()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }


        public IActionResult SearchLocation()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }

        public IActionResult SearchResults(string addressString, string livestockType, string priceRange)
        {
            
            _logger.LogInformation(addressString);

            ViewData["addressString"] = addressString;



            if (!IsSubscribed())
                return View("Subscription");
            else if (string.IsNullOrEmpty(addressString))
                return View("SearchLocation");
            else
            {
                var vendors = _context.AspNetUsers.Where(q => EF.Functions.Like(q.Address, string.Format("%{0}%", addressString))).ToList();
                return View(vendors);
            }
        }


        private bool IsSubscribed()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            if (loggedInUser.Subscribed == null || loggedInUser.Subscribed.ToLower() == "unsubscribed")
                return false;
            else
                return true;
        }

        
    }
}
