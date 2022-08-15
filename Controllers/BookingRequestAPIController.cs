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


        public IActionResult DeclineBookingRequestSubItem(int id) {


            var request = _context.BookingRequestSubItem.Where(q => q.Id == id).FirstOrDefault();

            request.Status = "Accepted";
            _context.SaveChanges();

            return Ok();
        }
        public IActionResult AcceptBookingRequestSubItem(int id)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var request = _context.BookingRequestSubItem.Where(q => q.Id == id).FirstOrDefault();

            var item = _context.MarketplaceItemLivestock
                           .Where(q => q.Id == request.SubItemId)
                           .FirstOrDefault();

            var bookingRequest = _context.BookingRequest
                            .Where(q => q.Id == request.BookingReqId)
                            .FirstOrDefault();


            request.Status = "Accepted";
            _context.SaveChanges();

           


            Transaction transaction = new Transaction()
            {
                Vendor = bookingRequest.Vendor,
                Farmer = loggedInUser.Id,
                Status = "Pending",
                TypeOfLivestock = item.Category,
                Quantity = request.Quantity,
                Kilos = item.Kilos,
                Amount = item.Price,
                BookDate = request.DeliveryDate,
                BookingReqSubitemId = id,
                DateCreated = DateTime.Now,
                Total = item.Unit.ToLower().Contains("kilo") ? (item.Kilos * item.Price) * (decimal)request.Quantity : (decimal)request.Quantity * item.Price
            };

            _context.Transaction.Add(transaction);
            _context.SaveChanges();

            ////deduct quantity to items
            item.Quantity = (int)item.Quantity - (int)request.Quantity;
            _context.MarketplaceItemLivestock.Update(item);
            _context.SaveChanges();

            DateTime d = (DateTime)request.DeliveryDate;
            d.AddHours(2);

            var vendor = _context.AspNetUsers.Where(q => q.Id == bookingRequest.Vendor).FirstOrDefault();
            var meatDealerName = vendor.Firstname + " " + vendor.LastName;

            string note = string.Format("Meat Dealer: {0}, Address: {1} ,Category:{2} , Quantity:{3}",meatDealerName, vendor.Address, transaction.TypeOfLivestock, request.Quantity);
            var schedule = new Schedules()
            {
                Vendor = bookingRequest.Vendor,
                BookingEndDate = request.DeliveryDate,
                BookingStartDate = d,
                Status = "Pending",
                Farmer = loggedInUser.Id,
                Notes = note,
                DateCreated = DateTime.Now
            };


            _context.Schedules.Add(schedule);
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


         
            /**
            * chat to the seller
            **/
            //select if there is a room where the logged in user and the vendor 
            var chatroom = getRoomID((int)bookingRequest.Vendor);
            if (chatroom == null)
            {
                //Created chatRoom
                Room room = new Room();
                _context.Room.Add(room); _context.SaveChanges();

                //add chatroom member
                var newMembers = new List<RoomMember> { new RoomMember { UserId = loggedInUser.Id, RoomId = room.Id }, new RoomMember { UserId = bookingRequest.Vendor, RoomId = room.Id } };
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

            return Ok();
        }

        public IActionResult GetBookingRequestFarmerv2(int id)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var data = (from req in _context.BookingRequest
                        join vendor in _context.AspNetUsers
                        on req.Vendor equals vendor.Id
                        join item in _context.MarketPlace
                        on req.Item equals item.Id
                        where req.Id == id
                        select new BookingRequestViewModelRevised
                        {
                            Id = req.Id,
                            Item = req.Item,
                            DateCreated = req.DateCreated,
                            Message = string.Format("{0} {1} booked you!", vendor.Firstname, vendor.LastName),
                            Address = item.Address,
                            Long = item.Lng,
                            Lat = item.Lat,
                            Status = item.Status,
                            SubItems = _context.BookingRequestSubItem
                                        .Where(q => q.MarketPlaceItem == req.Item)
                                        .Join(
                                            _context.MarketplaceItemLivestock, e =>
                                            e.SubItemId, d => d.Id, (e, d)
                                            => new BookingSubItems
                                            {
                                                QuantityBooked = e.Quantity,
                                                Unit = d.Unit,
                                                Category = d.Category,
                                                Price = d.Price,
                                                Amount = d.Unit.ToLower().Contains("kilo") ? (d.Kilos * d.Price) * (decimal)e.Quantity : (decimal)e.Quantity * d.Price,
                                                Kilos = d.Kilos
                                            }
                                        ).ToList()

                        }).FirstOrDefault();

            return Ok(data);
        }

        public IActionResult sampleGetData(int id) {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var data = (from req in _context.BookingRequest
                        join vendor in _context.AspNetUsers
                        on req.Vendor equals vendor.Id
                        join item in _context.MarketPlace
                        on req.Item equals item.Id
                        where req.Id == id
                        select new BookingRequestViewModelRevised
                        {
                            Id = req.Id,
                            Item = req.Item,
                            DateCreated = req.DateCreated,
                            Message = string.Format("{0} {1} booked you!", vendor.Firstname, vendor.LastName),
                            Address = item.Address,
                            Long = item.Lng,
                            Lat = item.Lat,
                            Status = item.Status,
                            SubItems = _context.BookingRequestSubItem
                                        .Where(q => q.MarketPlaceItem == req.Item)
                                        .Join(
                                            _context.MarketplaceItemLivestock, e => 
                                            e.SubItemId ,d => d.Id, (e, d) 
                                            => new BookingSubItems {
                                                QuantityBooked = e.Quantity,
                                                Unit = d.Unit,
                                                Category = d.Category,
                                                Price = d.Price,
                                                Amount = d.Price * e.Quantity
                                            }
                                        )
                                        
                                        .ToList()

                        }).FirstOrDefault();

            return Ok(data);
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
                            BookingQuantity = (int)req.Quantity,
                            Kilos = (int)item.Kilos,
                            Amount = (decimal)item.Amount,
                            TotalAmount = (decimal)item.Amount * (decimal)req.Quantity,
                            DateCreated = req.DateCreated,
                            Message = string.Format("{0} {1} booked you!", vendor.Firstname, vendor.LastName),
                            Address = item.Address,
                            Long  = item.Lng,
                            Lat = item.Lat,
                            Status = item.Status
                        }).FirstOrDefault();

            return Ok(data);
        }


        public IActionResult AddBookingRequestv2(Guid item, string itemsList)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            var x = _context.MarketPlace.Where(q => q.Id == item).FirstOrDefault();
            var farmer = _context.AspNetUsers.Where(q => q.Id == x.User).FirstOrDefault();

            List<BookingRequestDetails> items = new List<BookingRequestDetails>();
            JsonConvert.PopulateObject(itemsList, items);

            BookingRequest bookingRequest = new BookingRequest()
            {
                Item = item,
                Vendor = loggedInUser.Id,
                Farmer = farmer.Id,
                //Quantity = quantity,
                DateCreated = DateTime.Now
            };

            _context.BookingRequest.Add(bookingRequest);
            _context.SaveChanges();

            //save booking request details
            List<BookingRequestSubItem> subItems = new List<BookingRequestSubItem>();
            
            foreach (var subItem in items)
            {
                subItems.Add(new BookingRequestSubItem {
                    BookingReqId = bookingRequest.Id,
                    MarketPlaceItem = item,
                    SubItemId = subItem.ItemId,
                    Quantity = subItem.Quantity,
                    DeliveryDate = subItem.DeliveryDate
                });
            }

            _context.BookingRequestSubItem.AddRange(subItems);
            _context.SaveChanges();


            Notif notif = new Notif()
            {
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

        public IActionResult AcceptOrDeclineBookingv2(string status, int id, Guid notificationId)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var bookingRequest = _context.BookingRequest.Where(q => q.Id == id).FirstOrDefault();

            if (status.ToLower() == "accepted")
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
                    //TypeOfLivestock = item.Category,
                    //Quantity = bookingRequest.Quantity,
                    //Kilos = item.Kilos,
                    //Amount = bookingRequest.Quantity * item.Amount,
                    BookDate = bookingRequest.DateUpdated,
                    BookingId = item.Id,
                    DateCreated = DateTime.Now,
                };

                _context.Transaction.Add(transaction);
                _context.SaveChanges();

                //transaction details
                // - get bookingrequestsubitem
                var bookingReqSubItem = _context.BookingRequestSubItem.Where(q => q.BookingReqId == id).ToList();

                List<MarketplaceItemLivestock> marketPlaceSubItemToUpdate = new List<MarketplaceItemLivestock>();
                List<TransactionSubItem> transactionSubItems = new List<TransactionSubItem>();

                decimal totalAmount = 0;
                foreach (var subItem in bookingReqSubItem)
                {
                    var tempsubitem = _context.MarketplaceItemLivestock.Where(q => q.Id == subItem.SubItemId).FirstOrDefault();
                    //add new obj
                    transactionSubItems.Add(new TransactionSubItem
                    {
                        TransactionId = transaction.Id,
                        SubItemId = subItem.SubItemId,
                        Units = tempsubitem.Unit,
                        Category = tempsubitem.Category,
                        Kilos = tempsubitem.Kilos,
                        Price = tempsubitem.Price,
                        Quantity = subItem.Quantity,
                        SubTotal = tempsubitem.Unit.ToLower().Contains("kilo") ? (tempsubitem.Kilos * tempsubitem.Price) * (decimal)subItem.Quantity : (decimal)subItem.Quantity * tempsubitem.Price
                    });

                    //subItem.Quantity * tempsubitem.Price //
                    var whatToAdd = (tempsubitem.Unit.ToLower().Contains("kilo") ? (tempsubitem.Kilos * tempsubitem.Price) * (decimal)subItem.Quantity : (decimal)subItem.Quantity * tempsubitem.Price);
                    totalAmount = totalAmount + (decimal)whatToAdd;

                    //update obj esp. the qty minus auto
                    tempsubitem.Quantity = tempsubitem.Quantity - subItem.Quantity;
                    marketPlaceSubItemToUpdate.Add(tempsubitem);

                }

                _context.TransactionSubItem.AddRange(transactionSubItems);
                _context.SaveChanges();

                _context.MarketplaceItemLivestock.UpdateRange(marketPlaceSubItemToUpdate);
                _context.SaveChanges();

                transaction.Amount = totalAmount;
                _context.Transaction.Update(transaction);
                _context.SaveChanges();



                //item.Quantity = (int)item.Quantity - (int)bookingRequest.Quantity;
                //item.Total = item.Quantity * item.Amount;
                //_context.MarketPlace.Update(item);
                //_context.SaveChanges();


                /**
                 * chat to the seller
                 **/
                //select if there is a room where the logged in user and the vendor 
                var chatroom = getRoomID((int)bookingRequest.Vendor);
                if (chatroom == null)
                {
                    //Created chatRoom
                    Room room = new Room();
                    _context.Room.Add(room); _context.SaveChanges();

                    //add chatroom member
                    var newMembers = new List<RoomMember> { new RoomMember { UserId = loggedInUser.Id, RoomId = room.Id }, new RoomMember { UserId = bookingRequest.Vendor, RoomId = room.Id } };
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
                    Quantity = bookingRequest.Quantity,
                    Kilos = item.Kilos,
                    Amount = bookingRequest.Quantity * item.Amount,
                    BookDate = bookingRequest.DateUpdated,
                    BookingId = item.Id,
                    DateCreated = DateTime.Now
                };

                _context.Transaction.Add(transaction);
                _context.SaveChanges();

                item.Quantity = (int)item.Quantity - (int)bookingRequest.Quantity;
                item.Total = item.Quantity * item.Amount;
                _context.MarketPlace.Update(item);
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

    class BookingRequestDetails
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime DeliveryDate { get; set; }

    }
}
