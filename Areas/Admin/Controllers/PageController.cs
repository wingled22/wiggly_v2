﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class PageController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SubsReq()
        {
            return View();
        }

        public IActionResult ApprovedSubscription()
        {
            return View();
        }

        public IActionResult PendingSubscription()
        {
            return View();
        }


    }
}
