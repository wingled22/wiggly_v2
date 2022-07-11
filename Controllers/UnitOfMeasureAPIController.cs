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
    public class UnitOfMeasureAPIController : Controller
    {
        private readonly ILogger<UnitOfMeasureAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public UnitOfMeasureAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<UnitOfMeasureAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }
        public IActionResult GetUnits()
        {
            List<UnitsNames> lstock = _context.UnitOfMeasure.Select(x => new UnitsNames { Name = x.Name }).ToList();
            return Ok(lstock);
        }
    }

    public class UnitsNames
    {
        public string Name { get; set; }
    }
}
