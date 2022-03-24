using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Vendor.Controllers
{
    public class PostControlller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
