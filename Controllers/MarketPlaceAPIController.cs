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
using Wiggly.Models;

namespace Wiggly.Controllers
{
    public class MarketPlaceAPIController : Controller
    {
        private readonly ILogger<MarketPlaceAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public MarketPlaceAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<MarketPlaceAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetItems() {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var posts = (from item in _context.MarketPlace
                         join user in _context.AspNetUsers
                         on item.User equals user.Id
                         orderby item.DateCreated descending

                         select new MarketPlaceViewModel
                         {
                             ItemID = item.Id,
                             UserId = user.Id,
                             UserFullname = string.Format("{0} {1}", user.Firstname, user.LastName),
                             Caption = item.Caption,
                             BuyOrSell = item.BuyOrSell,
                             DateCreated = item.DateCreated.ToString(),
                             Category = item.Category,
                             ImageList = _context.PostPhoto.Where(p => item.Id == p.Post)
                                                .Select(p => new MarketPlaceImage { ImageId = p.Id, ImagePath = p.Path })
                                                .ToList(),
                             //Liked = _context.UserLikedPost.Any(q => q.Post == item.Id && q.User == loggedInUser.Id),
                             IsEditable = loggedInUser.Id == user.Id ? true : false

                         }).ToList();

            return Ok(posts);
        }

        [HttpGet]
        public IActionResult GetItem(Guid itemId)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var posts = (from item in _context.MarketPlace
                         join user in _context.AspNetUsers
                         on item.User equals user.Id
                         join photos in _context.PostPhoto
                         on item.Id equals photos.Post
                         orderby item.DateCreated descending
                         where item.Id == itemId
                         select new MarketPlaceViewModel
                         {
                             ItemID = item.Id,
                             UserId = user.Id,
                             UserFullname = string.Format("{0} {1}", user.Firstname, user.LastName),
                             Caption = item.Caption,
                             BuyOrSell = item.BuyOrSell,
                             Category = item.Category,
                             Description = item.Description,
                             DateCreated = item.DateCreated.ToString(),
                             ImageList = _context.PostPhoto.Where(p => item.Id == p.Post && p.Path.Contains("marketplace"))
                                                .Select(p => new MarketPlaceImage { ImageId = p.Id, ImagePath = p.Path })
                                                .ToList(),
                             //Liked = _context.UserLikedPost.Any(q => q.Post == item.Id && q.User == loggedInUser.Id),
                             IsEditable = loggedInUser.Id == user.Id ? true : false

                         }).FirstOrDefault();

            return Ok(posts);
        }



        [HttpPost]
        public IActionResult SaveItem(string values, string postImages)
        {
            if (string.IsNullOrEmpty(values))
                return BadRequest("Value sent is empty");

            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var val = new MarketPlaceViewModel();
            JsonConvert.PopulateObject(values, val);

            if (!TryValidateModel(val))
                return BadRequest("values sent error");

            //saving post
            var newItem = new MarketPlace()
            {
                Id = Guid.NewGuid(),
                Caption = val.Caption,
                Description = val.Description,
                User = loggedInUser.Id,
                DateCreated = DateTime.Now,
                BuyOrSell = val.BuyOrSell,
                Category = val.Category
            };

            if (!TryValidateModel(newItem))
                return BadRequest("Error adding item");

            _context.MarketPlace.Add(newItem);
            _context.SaveChanges();


            //saving post photo
            var listOfImages = new List<MarketPlaceImage>();
            JsonConvert.PopulateObject(postImages, listOfImages);
            if (listOfImages.Count > 0)
            {
                List<PostPhoto> postPhotos = new List<PostPhoto>();
                foreach (var image in listOfImages)
                {
                    //split the string to differentiate the filename
                    string path = image.ImagePath;
                    string[] subs = path.Split('/');

                    var temp = new PostPhoto()
                    {
                        Id = Guid.NewGuid(),
                        Path = image.ImagePath,
                        Filename = subs[3],
                        User = loggedInUser.Id,
                        Post = newItem.Id

                    };

                    postPhotos.Add(temp);

                }

                _context.PostPhoto.AddRange(postPhotos);
                _context.SaveChanges();
            }


            return Ok();
        }

        [HttpPost]
        public ActionResult UpdateItem(Guid key, string values, string postImages)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            //update post
            var item = _context.MarketPlace.Where(q => q.Id == key).FirstOrDefault();
            if (item == null || string.IsNullOrEmpty(values))
                return BadRequest("Post not found");

            var edited = new MarketPlaceViewModel();
            JsonConvert.PopulateObject(values, edited);
            item.Caption = edited.Caption;
            item.Description = edited.Description;
            item.BuyOrSell = edited.BuyOrSell;
            item.Category = edited.Category;

            _context.MarketPlace.Update(item);
            _context.SaveChanges();

            //updateimages
            //delete the images not on the postimages
            var listOfImagesToDelete = new List<MarketPlaceImage>();
            var listOfImagesFromSent = new List<MarketPlaceImage>();
            var listOfImagesFromDB = _context.PostPhoto.Where(q => q.Post == key)
                .Select(q => new MarketPlaceImage
                {
                    ImagePath = q.Path
                }).ToList();
            JsonConvert.PopulateObject(postImages, listOfImagesFromSent);

            //listOfImagesToAdd = listOfImagesFromDB.Except(listOfImagesFromSent,new ImagePathComparer()).ToList();
            var listOfImagesToAdd = listOfImagesFromSent.Select(q => q.ImagePath).Except(listOfImagesFromDB.Select(y => y.ImagePath)).ToList();

            if (listOfImagesToAdd.Count > 0)
            {
                List<PostPhoto> postPhotos = new List<PostPhoto>();
                foreach (var image in listOfImagesToAdd)
                {
                    //split the string to differentiate the filename
                    string path = image;
                    string[] subs = path.Split('/');

                    var temp = new PostPhoto()
                    {
                        Id = Guid.NewGuid(),
                        Path = image,
                        Filename = subs[3],
                        User = loggedInUser.Id,
                        Post = key

                    };

                    postPhotos.Add(temp);

                }
                _context.PostPhoto.AddRange(postPhotos);
                _context.SaveChanges();
            }

            return Ok();
        }

        public IActionResult DeleteItemPhoto(Guid image)
        {
            _logger.LogInformation(image.ToString());

            if (image == null)
                return BadRequest("Image id didn't exist.");

            var postPhoto = _context.PostPhoto.Where(q => q.Id == image).FirstOrDefault();
            _context.PostPhoto.Remove(postPhoto);
            _context.SaveChanges();

            return Ok();
        }


    }
}
