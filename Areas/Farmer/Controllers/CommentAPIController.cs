using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class CommentAPIController : Controller
    {
        private readonly ILogger<CommentAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public CommentAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<CommentAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        public ActionResult GetCommentsFromPost(Guid postId)
        {
            var comments = (from comment in _context.PostComment
                            join user in _context.AspNetUsers
                            on comment.User equals user.Id
                            orderby comment.DateCreated ascending
                            where comment.PostId == postId

                            select new CommentViewModel
                            {
                                Id = comment.Id,
                                UserId = user.Id,
                                UserFullname = string.Format("{0} {1}", user.Firstname, user.LastName),
                                CommentBody = comment.Comment,
                                DateCreated = ((DateTime)comment.DateCreated).ToString("MM/dd/yyyy HH:mm"),

                            }).ToList();
            if (comments != null)
                return Ok(comments);
            return Ok();
        }

        //public ActionResult PostComment(PostComment comment) {
        //    var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

        //    var newComment = new PostComment();
        //    newComment.Id = Guid.NewGuid();
        //    newComment.User = loggedInUser.Id;
        //    newComment.DateCreated = DateTime.Now;
        //    newComment.PostId = comment.PostId;
        //    newComment.Comment = comment.Comment;

        //    if (!TryValidateModel(newComment))
        //        return BadRequest("error on the data sent");

        //    _context.PostComment.Add(newComment);
        //    _context.SaveChanges();
        //    return Ok();
        //}
        public ActionResult PostComment(Guid postId, string userComment)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var comment = new PostComment()
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                Comment = userComment,
                User = loggedInUser.Id,
                DateCreated = DateTime.Now
            };

            if (!TryValidateModel(comment))
                return BadRequest("error on the data sent");

            _context.PostComment.Add(comment);
            _context.SaveChanges();
            return Ok();
        }
    }
}
