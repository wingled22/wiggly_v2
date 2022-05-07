using AutoMapper;
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

namespace Wiggly.Controllers
{
    public class AdminAccountController : Controller
    {
        private readonly ILogger<AdminAccountController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        private readonly IMapper _mapper;
        public AdminAccountController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<AdminAccountController> logger, IMapper mapper)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AdminRegisterViewModel registration)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new AppUser
                    {
                        Email = registration.Email,
                        UserType = "Admin",
                        UserName = registration.UserName
                    };

                    var result = await _usrMngr.CreateAsync(user, registration.Password);

                    if (result.Succeeded)
                    {
                        
                        _usrMngr.AddToRoleAsync(user, "Admin").Wait();

                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            if (item.Code == "DuplicateUserName")
                                ModelState.AddModelError("DuplicateUserName", "Username already taken");
                            if (item.Code == "DuplicateEmail")
                                ModelState.AddModelError("DuplicateEmail", "Email already Registered");

                        }
                        View(registration);
                    }


                }
                return View(registration);
            }
            catch (Exception e)
            {

                return View(registration);
            }
        }
    }
}
