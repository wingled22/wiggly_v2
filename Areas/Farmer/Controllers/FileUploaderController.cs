using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Areas.Farmer.Controllers
{
    [Authorize(Roles = "Farmer")]
    [Area("Farmer")]
    public class FileUploaderController : Controller
    {
        IWebHostEnvironment _hostingEnvironment;
        public FileUploaderController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public ActionResult Upload()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult UploadImageFromPost()
        {
            string newFileName = "";
            try
            {
                var myFile = Request.Form.Files["myFile"];
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "upload/post");
                // Uncomment to save the file
                //if(!Directory.Exists(path))
                //    Directory.CreateDirectory(path);
                var uniqueID = Guid.NewGuid().ToString();
                newFileName = string.Format("{0}{1}", uniqueID, myFile.FileName);
                
                using (var fileStream = System.IO.File.Create(Path.Combine(path,newFileName)))
                {
                    myFile.CopyTo(fileStream);
                    
                    HttpContext.Session.SetString("currentFilePath", string.Format("/upload/post/{0}",newFileName));
                }

            }
            catch
            {
                Response.StatusCode = 400;
            }

            //if (string.IsNullOrEmpty(newFileName))
                return new EmptyResult();
            //else
                //return Ok(newFileName);
        }

        [HttpGet]
        public IActionResult GetUploadedImageFromPost()
        {
            string file = HttpContext.Session.GetString("currentFilePath").ToString();
            if (!string.IsNullOrEmpty(file))
                return Ok(file);
            else
                return BadRequest("no file upload");
        }


        [HttpPost]
        public IActionResult UploadProfilePic()
        {
            string newFileName = "";
            try
            {
                var myFile = Request.Form.Files["myFile"];
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "upload/profilepic");
                // Uncomment to save the file
                //if(!Directory.Exists(path))
                //    Directory.CreateDirectory(path);
                var uniqueID = Guid.NewGuid().ToString();
                newFileName = string.Format("{0}{1}", uniqueID, myFile.FileName);

                using (var fileStream = System.IO.File.Create(Path.Combine(path, newFileName)))
                {
                    myFile.CopyTo(fileStream);

                    HttpContext.Session.SetString("currentFilePathProfilePic", string.Format("/upload/profilepic/{0}", newFileName));
                }

            }
            catch
            {
                Response.StatusCode = 400;
            }

            //if (string.IsNullOrEmpty(newFileName))
            return new EmptyResult();
            //else
            //return Ok(newFileName);
        }

        [HttpGet]
        public IActionResult GetUploadProfilePic()
        {
            string file = HttpContext.Session.GetString("currentFilePathProfilePic").ToString();
            if (!string.IsNullOrEmpty(file))
                return Ok(file);
            else
                return BadRequest("no file upload");
        }



        [HttpPost]
        public IActionResult UploadSubscriptionProofPayment()
        {
            string newFileName = "";
            try
            {
                var myFile = Request.Form.Files["myFile"];
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "upload/subscription");
                // Uncomment to save the file
                //if(!Directory.Exists(path))
                //    Directory.CreateDirectory(path);
                var uniqueID = Guid.NewGuid().ToString();
                newFileName = string.Format("{0}{1}", uniqueID, myFile.FileName);

                using (var fileStream = System.IO.File.Create(Path.Combine(path, newFileName)))
                {
                    myFile.CopyTo(fileStream);

                    HttpContext.Session.SetString("currentFilePathSubscriptionProofPayment", string.Format("/upload/subscription/{0}", newFileName));
                }

            }
            catch
            {
                Response.StatusCode = 400;
            }

            //if (string.IsNullOrEmpty(newFileName))
            return new EmptyResult();
            //else
            //return Ok(newFileName);
        }

        [HttpGet]
        public IActionResult GetUploadSubscriptionProofPayment()
        {
            string file = HttpContext.Session.GetString("currentFilePathSubscriptionProofPayment").ToString();
            if (!string.IsNullOrEmpty(file))
                return Ok(file);
            else
                return BadRequest("no file upload");
        }


    }
}
