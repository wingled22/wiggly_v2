using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Controllers
{
    public class LivestockAPIController : Controller
    {
        private readonly ILogger<LivestockAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public LivestockAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<LivestockAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }
        public IActionResult GetLivestock()
        {
            List<LivestockNames> lstock = _context.LivestockType.Select(x => new LivestockNames { Name  = x.Name }).ToList();
            return Ok(lstock);
        }

    }

    public class LivestockNames
    {
        public string Name { get; set; }
    }
}
