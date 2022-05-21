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
    public class ProfileAPIController : Controller
    {
        private readonly ILogger<ProfileAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public ProfileAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<ProfileAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult GetInfo()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var info = _context.ProfileInfo.Where(q => q.UserId == loggedInUser.Id).FirstOrDefault();

            return Ok(info);
        }

        public ActionResult PostInfo(string values, string pp)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var info = _context.ProfileInfo.Where(q => q.UserId == loggedInUser.Id).FirstOrDefault();

            _logger.LogInformation(pp);
            if (info == null)
            {
                ProfileInfo prof = new ProfileInfo();
                JsonConvert.PopulateObject(values, prof);
                prof.UserId = loggedInUser.Id;

                if (pp != null)
                {
                    var pic = new Vendor.Controllers.PP();
                    JsonConvert.PopulateObject(pp, pic);
                    prof.ProfilePic = pic.ImagePath;

                    var profilePic = new ProfilePic();

                    //split the string to differentiate the filename
                    string path = prof.ProfilePic;
                    string[] subs = path.Split('/');

                    profilePic.FilePath = prof.ProfilePic;
                    profilePic.Name = subs[3];
                    profilePic.UserId = loggedInUser.Id;

                    _context.ProfilePic.Add(profilePic);
                    _context.SaveChanges();

                }

                _context.ProfileInfo.Add(prof);
                _context.SaveChanges();

                return Ok();
            }
            else
            {
                ProfileInfo prof = new ProfileInfo();
                JsonConvert.PopulateObject(values, prof);
                info.Title = prof.Title;
                info.Description = prof.Description;
                if (pp != null)
                {
                    var pic = new Vendor.Controllers.PP();
                    
                    JsonConvert.PopulateObject(pp, pic); _logger.LogInformation("ang na convert:" + pic.ToString());
                    info.ProfilePic = pic.ImagePath;
                    //split the string to differentiate the filename
                    string fpath = pic.ImagePath; _logger.LogInformation("pic.ImagePath :" + pic.ImagePath); _logger.LogInformation("prof.ProfilePic :" + prof.ProfilePic);
                    string[] subs = fpath.Split('/');
                    var profilePic = _context.ProfilePic.Where(q => q.UserId == loggedInUser.Id).FirstOrDefault();
                    _logger.LogInformation("pic.ImagePath :" + pic.ImagePath);
                    _logger.LogInformation("1:" + subs[1]);
                    _logger.LogInformation("2:" + subs[2]);
                    _logger.LogInformation("3:" + subs[3]);

                    profilePic.FilePath = pic.ImagePath;
                    profilePic.Name = subs[3];
               

                    _context.ProfilePic.Update(profilePic);
                    _context.SaveChanges();
                }

                _context.ProfileInfo.Update(info);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest("somehing happened!");

        }
    }

    //class PP
    //{
    //    public string ImagePath { get; set; }
    //}
}
