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
        
        private void DeleteOldBookingNotif(Guid id)
        {
            var b = _context.Notif.Where(q => q.Id == id).FirstOrDefault();
            _context.Notif.Remove(b);
            _context.SaveChanges();
        }

    }
}
