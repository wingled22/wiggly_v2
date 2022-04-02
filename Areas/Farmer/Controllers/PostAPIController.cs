using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Areas.Farmer.Models;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Areas.Farmer.Controllers
{
    [Authorize(Roles = "Farmer")]
    [Area("Farmer")]
    public class PostAPIController : Controller
    {

        private readonly ILogger<PostAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public PostAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<PostAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        public ActionResult GetPost(Guid postId)
        {
            var posts = (from post in _context.Post
                         join user in _context.AspNetUsers
                         on post.User equals user.Id
                         join photos in _context.PostPhoto
                         on post.Id equals photos.Post
                         orderby post.DateCreated descending
                         where post.Id == postId
                         select new PostViewModel
                         {
                             Id = post.Id,
                             UserId = user.Id,
                             UserFullname = string.Format("{0} {1}", user.Firstname, user.LastName),
                             PostBody = post.Text,
                             DateCreated = (DateTime)post.DateCreated,
                         }).FirstOrDefault();

            return Ok(posts);
        }

        public ActionResult GetPosts()
        {
            var posts = (from post in _context.Post
                         join user in _context.AspNetUsers
                         on post.User equals user.Id
                         //join photos in _context.PostPhoto
                         //on post.Id equals photos.Post into phtos 
                         // from pg in phtos.GroupBy(c => c.Post, (key, c) => c.Select(s=> new Images { ImagePath = s.Path}).ToList() )
                          

                         orderby post.DateCreated descending

                         select new PostViewModel {
                             Id = post.Id,
                             UserId = user.Id,
                             UserFullname = string.Format("{0} {1}", user.Firstname, user.LastName),
                             PostBody = post.Text,
                             DateCreated = (DateTime)post.DateCreated,
                             ImageList = _context.PostPhoto.Where(p => post.Id == p.Post)
                                                .Select(p => new Images { ImagePath = p.Path })
                                                .ToList()
                         }).ToList();

            return Ok(posts);
        }

        public ActionResult SavePost(string values, string postImages)
        {
            if (string.IsNullOrEmpty(values))
                return BadRequest("Value sent is empty");

            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var val = new PostViewModel();
            JsonConvert.PopulateObject(values, val);

            if (!TryValidateModel(val))
                return BadRequest("values sent error");

            
            //saving post
            var newPost = new Post()
            {
                Id = Guid.NewGuid(),
                Text = val.PostBody,
                User = loggedInUser.Id,
                DateCreated = DateTime.Now
            };

            if (!TryValidateModel(newPost))
                return BadRequest("Error posting");

            _context.Post.Add(newPost);
            _context.SaveChanges();



            //saving post photo
            var listOfImages = new List<Images>();
            JsonConvert.PopulateObject(postImages, listOfImages);
            if (listOfImages.Count > 0)
            {
                List<PostPhoto> postPhotos = new List<PostPhoto>();
                foreach (var image in listOfImages)
                {
                    //split the string to differentiate the filename
                    string path = image.ImagePath;
                    string[] subs = path.Split('/');

                    var temp = new PostPhoto() {
                        Id = Guid.NewGuid(),
                        Path = image.ImagePath,
                        Filename = subs[3],
                        User = loggedInUser.Id,
                        Post = newPost.Id

                    };

                    postPhotos.Add(temp);

                }

                _context.PostPhoto.AddRange(postPhotos);
                _context.SaveChanges();
            }


            return Ok();
        }
    }
}
