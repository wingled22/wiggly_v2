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

namespace Wiggly.Areas.Farmer.Controllers
{
    [Authorize(Roles = "Farmer")]
    [Area("Farmer")]
    public class SubscriptionAPIController : Controller
    {
        private readonly ILogger<SubscriptionAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public SubscriptionAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<SubscriptionAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        public ActionResult PostRequest(int months, string pp)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var info = _context.ProfileInfo.Where(q => q.UserId == loggedInUser.Id).FirstOrDefault();

            _logger.LogInformation(months.ToString());
            _logger.LogInformation(months.GetType().ToString());
            _logger.LogInformation(pp);

            if (string.IsNullOrEmpty(pp))
            {
                BadRequest("something happened processing your request, refresh the browser and try again");
            }
            else
            {
                var proofOfPayment = new SubsPic();
                JsonConvert.PopulateObject(pp, proofOfPayment);

                SubscriptionRequest subscriptionRequest = new SubscriptionRequest()
                {
                    UserId = loggedInUser.Id,
                    Date = DateTime.Now,
                    ProofOfPayment = proofOfPayment.ImagePath,
                    Months = months
                };

                _context.SubscriptionRequest.Add(subscriptionRequest);
                _context.SaveChanges();
            }

            return Ok();
        }

        public IActionResult GetPendingRequest()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var pendingReq = _context.SubscriptionRequest.Where(q => q.UserId == loggedInUser.Id && q.Status.ToLower() == "pending").OrderBy(q=>q.Date).FirstOrDefault();
            return Ok(pendingReq);
        }

       
    }

    class SubsPic
    {
        public string ImagePath { get; set; }
    }
}
