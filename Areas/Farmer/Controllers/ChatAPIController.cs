using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Entities;
using Wiggly.Hubs;
using Wiggly.Identity;

namespace Wiggly.Areas.Farmer.Controllers
{
    [Authorize(Roles = "Farmer")]
    [Area("Farmer")]
    public class ChatAPIController : Controller
    {
        private readonly ILogger<ChatAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public ChatAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<ChatAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }
        

        public string GetRoom(int vendorID)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();


            var roomID = (from member in _context.RoomMember
                               join room in _context.Room on member.RoomId equals room.Id
                               join me in _context.RoomMember on room.Id equals me.RoomId

                               where member.UserId == vendorID && me.UserId == loggedInUser.Id
                               //select new Room
                               //{
                               //    Id = room.Id,
                               //    RoomName = member.UserId.ToString()
                               //}
                               select room.Id.ToString()
                              ).FirstOrDefault();

            if (string.IsNullOrEmpty(roomID))
            {
                //Create chatRoom
                Room room = new Room();
                _context.Room.Add(room); _context.SaveChanges();

                //add chatroom member
                var newMembers = new List<RoomMember> { new RoomMember { UserId = loggedInUser.Id, RoomId = room.Id }, new RoomMember { UserId = vendorID, RoomId = room.Id } };
                _context.RoomMember.AddRange(newMembers);
                _context.SaveChanges();

                var newMessage = new Message()
                {
                    Id = Guid.NewGuid(),
                    Room = room.Id,
                    UserId = loggedInUser.Id,
                    MessageText = "Good day, Already accepted your booking request!",
                    DatetimeCreate = DateTime.Now
                };

                roomID = room.Id.ToString();
            }

            return (roomID);
        }

        public IActionResult GetRoomMessages(int roomID)
        {
            _logger.LogDebug("getting room messages");
            _logger.LogInformation(string.Format("type of roomID: {0}", roomID.GetType()));
            List<RoomMessage> roomMessages = new List<RoomMessage>();
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();


            var res = (from message in _context.Message
                       join user in _context.AspNetUsers
                       on message.UserId equals user.Id
                       orderby message.DatetimeCreate ascending
                       where message.Room == roomID
                       select new RoomMessage
                       {
                           MessageId = message.Id,
                           Room = (int)message.Room,
                           MessageText = message.MessageText,
                           IsSender = message.UserId == loggedInUser.Id ? true : false,
                           SenderFullname = string.Format("{0} {1}", user.Firstname, user.LastName)
                       }
                ).ToList();
            return Ok(res);
        }

        public IActionResult SendMessage(int roomId, string message)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var recipient = _context.RoomMember.Where(q => q.RoomId == roomId && q.UserId != loggedInUser.Id).FirstOrDefault();
            
            try
            {

                var newMessage = new Message()
                {
                    Id = Guid.NewGuid(),
                    Room = roomId,
                    UserId = loggedInUser.Id,
                    MessageText = message,
                    DatetimeCreate = DateTime.Now
                };

                _context.Message.Add(newMessage);
                _context.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();

            }
        }

    }
}
