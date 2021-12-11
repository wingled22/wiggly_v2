using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        private readonly IMapper _mapper;
        public AccountController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<AccountController> logger, IMapper mapper)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        // GET: AccountController
        public ActionResult Index()
        {
            return View("Register");
        }

        public ActionResult Register() {
            if (User.Identity.IsAuthenticated)
            {
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                if (currentUser.IsInRole("Vendor"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Vendor" });
                }
                if (currentUser.IsInRole("Farmer"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Farmer" });
                }
            }
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registration)
        {
            try {
                if (ModelState.IsValid)
                {
                    var user = new AppUser
                    {
                        Firstname = registration.FirstName,
                        MiddleName = registration.MiddleName,
                        LastName = registration.LastName,
                        Contact = registration.ContactNumber,
                        UserType = registration.UserType,
                        Email = registration.Email,
                        UserName = registration.UserName
                    };

                    var result = await _usrMngr.CreateAsync(user, registration.Password);

                    if (result.Succeeded)
                    {
                        if (user.UserType == "Farmer")
                            _usrMngr.AddToRoleAsync(user, "Farmer").Wait();
                        else
                            _usrMngr.AddToRoleAsync(user, "Vendor").Wait();

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
            catch(Exception e)
            {

                return View(registration);
            }
        }

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                if (currentUser.IsInRole("Vendor"))
                {
                    return RedirectToAction("Index", "Vendor", new { area = "Vendor" });
                }
                if (currentUser.IsInRole("Farmer"))
                {
                    return RedirectToAction("Index", "Farmer", new { area = "Farmer" });
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = _context.AspNetUsers.Where(q => q.Email == login.Email).FirstOrDefault();
                var appUser = _mapper.Map<AppUser>(user);
                if(user != null)
                {
                    var result = await _signInMngr.PasswordSignInAsync(appUser, login.Password, false, false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        if (user.UserType == "Farmer")
                        {
                            return RedirectToAction("Index", "Home", new { area = "Farmer"});
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home", new { area = "Vendor" });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("passwordAndEmailNotMatch ", "Email and password didn't match");
                        return View(login);
                    }
                }
                else
                {
                    ModelState.AddModelError("noEmailFind ", "No email found.");
                    return View(login);
                }
            }
            return View();
        }

        [Route("Account/Logout")]
        [Route("Vendor/Account/Logout")]
        [Route("Farmer/Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInMngr.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public JsonResult isUserNameExist(string UserName)
        {
            var user = _context.AspNetUsers.Where(q => q.UserName == UserName).FirstOrDefault();
            return Json(user == null);
        }

        [HttpPost]
        public JsonResult isEmailExist(string Email)
        {
            var user = _context.AspNetUsers.Where(q => q.Email == Email).FirstOrDefault();
            return Json(user == null);
        }
    }
}
