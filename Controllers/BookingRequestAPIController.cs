using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Entities;
using Wiggly.Identity;
using Wiggly.Models;

namespace Wiggly.Controllers
{
    public class BookingRequestAPIController : Controller
    {
        private readonly ILogger<BookingRequestAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public BookingRequestAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<BookingRequestAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }


        public IActionResult GetBookingRequestFarmer(int id)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var data = (from req  in _context.BookingRequest
                        join vendor in _context.AspNetUsers
                        on req.Vendor equals vendor.Id
                        join item in _context.MarketPlace
                        on req.Item equals item.Id
                        where req.Id == id
                        select new BookingRequestViewModel
                        {
                            Id = req.Id,
                            Item = req.Item,
                            Category = item.Category,
                            Quantity = (int)item.Quantity,
                            Kilos = (int)item.Kilos,
                            Amount = (decimal)item.Amount,
                            DateCreated = req.DateCreated,
                            Message = string.Format("{0} {1} booked you!", vendor.Firstname, vendor.LastName),
                            Address = item.Address,
                            Long  = item.Lng,
                            Lat = item.Lat,
                            Status = item.Status
                        }).FirstOrDefault();

            return Ok(data);
        }

        public IActionResult AddBookingRequest(Guid item)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var x = _context.MarketPlace.Where(q => q.Id == item).FirstOrDefault();
            var farmer = _context.AspNetUsers.Where(q => q.Id == x.User).FirstOrDefault(); 

            BookingRequest bookingRequest = new BookingRequest() { 
                Item = item,
                Vendor = loggedInUser.Id,
                Farmer = farmer.Id,
                DateCreated = DateTime.Now
            };

            _context.BookingRequest.Add(bookingRequest);
            _context.SaveChanges();


            Notif notif = new Notif() {
                Id = Guid.NewGuid(),
                Recipient = x.User,
                Message = string.Format("{0} {1} booked you", loggedInUser.Firstname, loggedInUser.LastName),
                DateCreated = DateTime.Now,
                DateCreatedString = DateTime.Now.ToString("MMMM dd, yyyy"),
                NotifType = "Booking",
                BookingRequest = bookingRequest.Id
            };

            _context.Notif.Add(notif);
            _context.SaveChanges();

            return Ok();
        }

        public IActionResult UpdateBookingRequest(int item, string status)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var bookingRequest = _context.BookingRequest.Where(q =>q.Id == item).FirstOrDefault();
            bookingRequest.Status = status;
            bookingRequest.DateUpdated = DateTime.Now;

            //Todo: notify to the dealer if if the user accepted or declined the booking
            _context.SaveChanges();
            return Ok();
        }


        public IActionResult AcceptOrDeclineBooking(string status, int id, Guid notificationId)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var bookingRequest = _context.BookingRequest.Where(q => q.Id == id).FirstOrDefault();

            if(status.ToLower() == "accepted")
            {
                bookingRequest.Status = "Accepted";
                bookingRequest.DateUpdated = DateTime.Now;
                _context.BookingRequest.Update(bookingRequest);
                _context.SaveChanges();

                //add a new notif
                Notif notif = new Notif()
                {
                    Id = Guid.NewGuid(),
                    Recipient = bookingRequest.Vendor,
                    Message = string.Format("{0} {1} accepted your booking", loggedInUser.Firstname, loggedInUser.LastName),
                    DateCreated = DateTime.Now,
                    DateCreatedString = DateTime.Now.ToString("MMMM dd, yyyy"),
                    NotifType = "Booking",
                    BookingRequest = bookingRequest.Id
                };

                _context.Notif.Add(notif);
                _context.SaveChanges();

                DeleteOldBookingNotif(notificationId);

                var item = _context.MarketPlace.Where(q => q.Id == bookingRequest.Item).FirstOrDefault();

                //create transaction 
                Transaction transaction = new Transaction()
                {
                    Vendor = bookingRequest.Vendor,
                    Farmer = loggedInUser.Id,
                    Status = "Pending",
                    TypeOfLivestock = item.Category,
                    Quantity = item.Quantity,
                    Kilos = item.Kilos,
                    Amount = item.Amount,
                    BookDate = bookingRequest.DateUpdated,
                    BookingId = item.Id,
                    DateCreated = DateTime.Now
                };

                _context.Transaction.Add(transaction);
                _context.SaveChanges();


                /**
                 * chat to the seller
                 **/
                //select if there is a room where the logged in user and the vendor 
                var chatroom = getRoomID((int)bookingRequest.Vendor);
                if(chatroom == null)
                {
                    //Created chatRoom
                    Room room = new Room();
                    _context.Room.Add(room); _context.SaveChanges();

                    //add chatroom member
                    var newMembers = new List<RoomMember> { new RoomMember{ UserId = loggedInUser.Id, RoomId = room.Id }, new RoomMember{ UserId = bookingRequest.Vendor, RoomId = room.Id } };
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

                    _context.Message.Add(newMessage);
                    _context.SaveChanges();
                }
                else
                {
                    //send message to the 
                    var newMessage = new Message()
                    {
                        Id = Guid.NewGuid(),
                        Room = chatroom.Id,
                        UserId = loggedInUser.Id,
                        MessageText = "Good day, Already accepted your booking request!",
                        DatetimeCreate = DateTime.Now
                    };

                    _context.Message.Add(newMessage);
                    _context.SaveChanges();
                }



            }
            else
            {
                bookingRequest.Status = "Declined";
                bookingRequest.DateUpdated = DateTime.Now;
                _context.BookingRequest.Update(bookingRequest);
                _context.SaveChanges();


                //add a new notif
                Notif notif = new Notif()
                {
                    Id = Guid.NewGuid(),
                    Recipient = bookingRequest.Vendor,
                    Message = string.Format("{0} {1} declined your booking", loggedInUser.Firstname, loggedInUser.LastName),
                    DateCreated = DateTime.Now,
                    DateCreatedString = DateTime.Now.ToString("MMMM dd, yyyy"),
                    NotifType = "Booking",
                    BookingRequest = bookingRequest.Id
                };

                DeleteOldBookingNotif(notificationId);


            }

            return Ok();
        }

        private Room getRoomID(int vendorID) {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            

            var isRoomExist = (from member in _context.RoomMember
                               join room in _context.Room on member.RoomId equals room.Id
                               join me in _context.RoomMember on room.Id equals me.RoomId

                               where member.UserId == vendorID && me.UserId == loggedInUser.Id
                               select new Room
                               {
                                   Id = room.Id,
                                   RoomName = member.UserId.ToString()
                               }
                              ).FirstOrDefault();

            return (isRoomExist);
        }

        private void DeleteOldBookingNotif(Guid id)
        {
            var b = _context.Notif.Where(q => q.Id == id).FirstOrDefault();
            _context.Notif.Remove(b);
            _context.SaveChanges();
        }

    }
}
