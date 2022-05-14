using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class RequestAPIController : Controller
    {
        private readonly ILogger<RequestAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public RequestAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<RequestAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetRequests()
        {
            var res = _context.SubscriptionRequest.OrderByDescending(q => q.Status).ToList();
            return Ok(res);
        }

        public IActionResult GetPendingRequests()
        {
            var res = _context.SubscriptionRequest.Where(q=> q.Status == "Pending").OrderByDescending(q => q.Status).ToList();
            return Ok(res);
        }

        public IActionResult GetApprovedRequests()
        {
            var res = _context.SubscriptionRequest.Where(q => q.Status == "Approved").OrderByDescending(q => q.Status).ToList();
            return Ok(res);
        }

        [HttpPut]
        public IActionResult UpdateRequest(int key, string values)
        {
            try
            {
                _logger.LogInformation(key.ToString());
                _logger.LogInformation(values);

                if(key == 0)
                    return BadRequest("data sent is null");

                if (values == null)
                    return BadRequest("data sent is null");
                else
                {
                    var up = new SubscriptionRequest();
                    JsonConvert.PopulateObject(values, up);
                    var req = _context.SubscriptionRequest.Where(q => q.Id == key).FirstOrDefault();
                    req.Status = up.Status;
                    req.StartSubs = DateTime.Now;
                    req.EndSubs = DateTime.Now.AddMonths((int)req.Months);

                    _context.SubscriptionRequest.Update(req);
                    _context.SaveChanges();

                    var user = _context.AspNetUsers.Where(q => q.Id == req.UserId).FirstOrDefault();

                    if(up.Status == "Approved")
                    {
                        user.Subscribed = "Subscribed";
                    }
                    else
                    {
                        user.Subscribed = "Unsubscribed";
                    }
                    _context.AspNetUsers.Update(user);
                    _context.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                return BadRequest("Something happened when executing the action");
            }
        }
    }
}
