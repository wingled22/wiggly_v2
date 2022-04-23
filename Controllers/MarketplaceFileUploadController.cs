using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wiggly.Controllers
{
    public class MarketplaceFileUploadController : Controller
    {
        IWebHostEnvironment _hostingEnvironment;
        public MarketplaceFileUploadController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public IActionResult UploadItemImage()
        {
            string newFileName = "";
            try
            {
                var myFile = Request.Form.Files["myFile"];
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "upload/marketplace");
                // Uncomment to save the file
                //if(!Directory.Exists(path))
                //    Directory.CreateDirectory(path);
                var uniqueID = Guid.NewGuid().ToString();
                newFileName = string.Format("{0}{1}", uniqueID, myFile.FileName);

                using (var fileStream = System.IO.File.Create(Path.Combine(path, newFileName)))
                {
                    myFile.CopyTo(fileStream);

                    HttpContext.Session.SetString("currentFilePath", string.Format("/upload/marketplace/{0}", newFileName));
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
        public IActionResult GetUploadedItemImage()
        {
            string file = HttpContext.Session.GetString("currentFilePath").ToString();
            if (!string.IsNullOrEmpty(file))
                return Ok(file);
            else
                return BadRequest("no file upload");
        }
    }
}
