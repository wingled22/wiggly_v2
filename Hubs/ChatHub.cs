using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Hubs
{
    //[Authorize]
    public class ChatHub :  Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly UserManager<AppUser> _usrMngr;
        private readonly SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;

        //make this static so that you can list all the connections
        private readonly static List<ConnectionMap> _ConnectionsMap = new List<ConnectionMap>();

        public ChatHub(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<ChatHub> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            try
            {
                //var usr = Context.User.Identity.Name;
                var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == Context.User.Identity.Name).FirstOrDefault();
                //userViewModel.Device = GetDevice();
                //userViewModel.CurrentRoom = "";

                if (!_ConnectionsMap.Any(u => u.userId == loggedInUser.Id))
                {
                    _logger.LogInformation(Context.ConnectionId);
                    var newConn = new ConnectionMap() { userId = loggedInUser.Id, connectionID = Context.ConnectionId, FullName = string.Format("{0} {1}", loggedInUser.Firstname, loggedInUser.LastName) };
                    _logger.LogInformation("added info to connection map");
                    _ConnectionsMap.Add(newConn);
                }
                else
                {
                    _logger.LogInformation("refreshed connectionID "+ Context.ConnectionId);
                     _ConnectionsMap.Where(q => q.userId == loggedInUser.Id).Select(q => { q.connectionID = Context.ConnectionId; return q; }).FirstOrDefault();
                    
                }

                Clients.Caller.SendAsync("onConnected");
            }
            catch (Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
            }
            return base.OnConnectedAsync();
        }

        public AspNetUsers getLoggedInUser()
        { 
            return _context.AspNetUsers.Where(q => q.UserName == Context.User.Identity.Name).FirstOrDefault();
        }

        public List<ConnectionMap> getAllConnections() {
            return _ConnectionsMap.ToList();
        }

        public int CreateRoom(Guid item)
        {
            _logger.LogInformation(string.Format("type of item is : {0}", item.GetType().ToString()));

            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == Context.User.Identity.Name).FirstOrDefault();


            //check if there is a room for this item and user
            var isRoomExist = _context.RoomMember.Any(q => q.UserId == loggedInUser.Id && q.ItemId == item);
            _logger.LogInformation(isRoomExist.ToString());

            //if true return the roomID
            if (isRoomExist) {
                var room = _context.RoomMember.Where(q => q.UserId == loggedInUser.Id && q.ItemId == item).FirstOrDefault();

                return (int)room.RoomId;
            }
            //if false create a room, add room members and return the RoomID
            else
            {
                var itm = _context.MarketPlace.Where(q => q.Id == item).FirstOrDefault();

                var newRoom = new Room() { RoomName = itm.Caption };
                _context.Room.Add(newRoom);
                _context.SaveChanges();

                var newMembers = new List<RoomMember>();
                newMembers.Add(new RoomMember { 
                    UserId = loggedInUser.Id,
                    RoomId = newRoom.Id,
                    ItemId = itm.Id
                });

                newMembers.Add(new RoomMember
                {
                    UserId = itm.User,
                    RoomId = newRoom.Id,
                    ItemId = itm.Id
                });


                _context.RoomMember.AddRange(newMembers);
                _context.SaveChanges();

                return newRoom.Id;
            }

        }


        public int CreatePersonalRoom(int userId)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == Context.User.Identity.Name).FirstOrDefault();
            _logger.LogInformation(string.Format("type of userId: {0}", userId.GetType()));



            var isUserHasAnyRoom = _context.RoomMember.ToList().Any(q => q.UserId == loggedInUser.Id);
            _logger.LogInformation(string.Format("isUserHasAnyRoom: {0}", isUserHasAnyRoom));
            if (!isUserHasAnyRoom)
            {

                var newRoom = new Room();
                _context.Room.Add(newRoom);
                _context.SaveChanges();

                var newMembers = new List<RoomMember>
                {
                    new RoomMember
                    {
                        UserId = loggedInUser.Id,
                        RoomId = newRoom.Id
                    },

                    new RoomMember
                    {
                        UserId = userId,
                        RoomId = newRoom.Id
                    }
                };
                _context.RoomMember.AddRange(newMembers);
                _context.SaveChanges();

                return newRoom.Id;
            }
            else
            {
                //check if there is a room for this item and user
                var isRoomExist = (from member in _context.RoomMember
                                   join room in _context.Room on member.RoomId equals room.Id
                                   join me in _context.RoomMember on member.RoomId equals me.RoomId

                                   where member.UserId == userId && member.ItemId == null && me.UserId == loggedInUser.Id && me.ItemId == null && room.RoomName == null
                                   select room.Id
                                   ).FirstOrDefault();
                _logger.LogInformation(isRoomExist.ToString());

                if (isRoomExist == 0)
                {
                    var newRoom = new Room();
                    _context.Room.Add(newRoom);
                    _context.SaveChanges();

                    var newMembers = new List<RoomMember>();
                    newMembers.Add(new RoomMember
                    {
                        UserId = loggedInUser.Id,
                        RoomId = newRoom.Id
                    });

                    newMembers.Add(new RoomMember
                    {
                        UserId = userId,
                        RoomId = newRoom.Id
                    });

                    _context.RoomMember.AddRange(newMembers);
                    _context.SaveChanges();
                    return newRoom.Id;
                }

                return isRoomExist;
            }

            
        }

        public RoomInfo GetRoomInfo(int roomId)
        {
            var room = _context.Room.Where(q => q.Id == roomId).FirstOrDefault();
            if(room == null)
            {
                _logger.LogInformation("room is null");
                return null;
            }
            else
            {
                _logger.LogInformation(room.ToString());
                var roomInfo = new RoomInfo() { RoomId = room.Id, RoomName = room.RoomName };
                return roomInfo;
            }
        }

        public string GetMyRecieversName (int roomId)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == Context.User.Identity.Name).FirstOrDefault();
            var reciever = _context.RoomMember.Where(q => q.RoomId == roomId && q.UserId != loggedInUser.Id).FirstOrDefault();
            
            if(reciever != null)
            {
                var recieverInfo = _context.AspNetUsers.Where(q => q.Id == reciever.UserId).FirstOrDefault();
                if(recieverInfo != null)
                {
                    return string.Format("[{0} {1}",recieverInfo.Firstname, recieverInfo.LastName);
                }
            }

            return "Reload to get reciever info";
        }
        

        public List<RoomMessage> GetRoomMessages(int roomId) {
            _logger.LogDebug("getting room messages");
            _logger.LogInformation(string.Format("type of roomID: {0}", roomId.GetType()));
            List<RoomMessage> roomMessages = new List<RoomMessage>();
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == Context.User.Identity.Name).FirstOrDefault();

            var res = (from message in _context.Message
                       join user in _context.AspNetUsers
                       on message.UserId equals user.Id
                       orderby message.DatetimeCreate ascending
                       where message.Room == roomId
                       select new RoomMessage
                       {
                           MessageId = message.Id,
                           Room = (int)message.Room,
                           MessageText = message.MessageText,
                           IsSender = message.UserId == loggedInUser.Id ? true : false,
                           SenderFullname = string.Format("{0} {1}", user.Firstname, user.LastName)
                       }
                ).ToList();

            return res;
        }

        public List<RoomInfoSorted> GetRoomsOfUser()
        {
            List<RoomInfoSorted> rooms = new List<RoomInfoSorted>();
            //_context.AspNetUsers.Where(q => q.Id == member.Id).Select(q => (string)string.Format("{0} {1}", q.Firstname, q.LastName)).FirstOrDefault()
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == Context.User.Identity.Name).FirstOrDefault();

            var res = (from room in _context.Room
                       join member in _context.RoomMember
                       on room.Id equals member.RoomId
                       join user in _context.AspNetUsers
                       on member.UserId equals user.Id
                       join msg in _context.Message
                       on room.Id equals msg.Room into info
                       from t in info.DefaultIfEmpty()

                       where user.Id == loggedInUser.Id
                       select new RoomInfoSorted
                       {
                           RoomId = (int)t.Room,
                           RoomName = room.RoomName == null ? _context.RoomMember.Join(
                               _context.AspNetUsers,
                               mmbr => mmbr.UserId,
                               usr => usr.Id,
                               (mmbr, usr) => new { mmbr, usr }

                           ).Where(q => q.mmbr.RoomId == room.Id && q.usr.Id != loggedInUser.Id).Select(q => (string)string.Format("{0} {1}", q.usr.Firstname, q.usr.LastName)).FirstOrDefault() : room.RoomName,
                           MessageCount = _context.Message.Where(q => q.Room == room.Id).Count(),
                           DateUpdated = _context.Message.Where(q => q.Room == room.Id).OrderByDescending(q => q.DatetimeCreate).Select(q => (DateTime)q.DatetimeCreate).FirstOrDefault()
                       }).OrderBy(q=>q.DateUpdated).ToList();

            var distincted = res.GroupBy(x => x.DateUpdated, (key, group) => group.First()).ToList();

            return distincted;
        }

        public bool SendMessageFromMarketPlace(int roomId, string message)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == Context.User.Identity.Name).FirstOrDefault();
            var recipient = _context.RoomMember.Where(q => q.RoomId == roomId && q.UserId != loggedInUser.Id).FirstOrDefault();
            var recipientConnId = _ConnectionsMap.Where(q => q.userId == recipient.UserId).FirstOrDefault();
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

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task  SendMessage(int roomId, string message)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == Context.User.Identity.Name).FirstOrDefault();
            var recipient = _context.RoomMember.Where(q => q.RoomId == roomId && q.UserId != loggedInUser.Id).FirstOrDefault();
            var recipientConnId = _ConnectionsMap.Where(q => q.userId == recipient.UserId).FirstOrDefault();
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

                
                if(recipientConnId == null)
                {
                    await Clients.Caller.SendAsync("refreshMe");

                }
                {
                    await Clients.Client(recipientConnId.connectionID).SendAsync("refreshMe");
                }
                //Clients.
            }
            catch
            {
                await Clients.Caller.SendAsync("refreshMe");

            }
        }


    }
    public class ConnectionMap
    {
        public string connectionID { get; set; }
        public int userId { get; set; }
        public string FullName { get; set; }
    }

    public class MyMarketplaceChatRoom
    {
        public int RoomId { get; set; }
        public int UserId{ get; set; }
    }

    public class RoomInfo
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
    }

    public class RoomInfoSorted
    {
        public int RoomId { get; set; }
        public int MessageCount { get; set; }
        public string RoomName { get; set; }
        public DateTime DateUpdated { get; set; }
        
    }

    public class RoomMessage
    {
        public Guid MessageId { get; set; }
        public bool IsSender { get; set; }
        public string SenderFullname{ get; set; }
        public int Room { get; set; }
        public string MessageText { get; set; }
    }
}
