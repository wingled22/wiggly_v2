using AutoMapper;
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
using Wiggly.ViewModels;

namespace Wiggly.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UsersAPIController : Controller
    {
        private readonly ILogger<UsersAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        private readonly IMapper _mapper;

        public UsersAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<UsersAPIController> logger, IMapper mapper)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetUsers()
        {
            var users = _context.AspNetUsers.Where(q => q.UserType != "Admin").ToList();
            return Ok(users);
        }

        public IActionResult GetUsersWithFullname()
        {
            var users = _context.AspNetUsers.Where(q => q.UserType != "Admin").ToList();
            var result = _mapper.Map<List<UsersWithFullname>>(users);
            return Ok(result);
        }


    }
}
