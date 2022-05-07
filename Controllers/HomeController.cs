using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Wiggly.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                if (currentUser.IsInRole("Vendor"))
                {
                    return RedirectToAction("Index", "Page", new { area = "Vendor" });
                }
                if (currentUser.IsInRole("Farmer"))
                {
                    return RedirectToAction("Index", "Page", new { area = "Farmer" });
                }
                if (currentUser.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Page", new { area = "Admin" });
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View();
        }
    }
}
