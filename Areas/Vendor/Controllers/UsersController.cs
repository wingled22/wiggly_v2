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

namespace Wiggly.Areas.Vendor.Controllers
{
    [Authorize(Roles = "Vendor")]
    [Area("Vendor")]
    public class UsersController : Controller
    {

        private readonly ILogger<UsersController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public UsersController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<UsersController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult GetVendors()
        {
            var res = _context.AspNetUsers.Where(q => q.UserType == "Vendor").ToList();
            if (res == null)
                BadRequest("No Vendor ");
            return Ok(res);
        }

        [HttpGet]
        public ActionResult GetFarmers()
        {
            var res = _context.AspNetUsers.Where(q => q.UserType == "Farmer").ToList();
            if (res == null)
                BadRequest("No Farmer ");
            return Ok(res);
        }
    }
}
