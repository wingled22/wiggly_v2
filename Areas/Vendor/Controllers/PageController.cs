using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Controllers;
using Wiggly.Entities;
using Wiggly.Identity;
using Wiggly.Models;

namespace Wiggly.Areas.Vendor.Controllers
{
    [Authorize(Roles = "Vendor")]
    [Area("Vendor")]
    public class PageController : Controller
    {
        private readonly ILogger<PageController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public PageController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<PageController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View("Index-v4");
        }

        public IActionResult Transaction()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }

        public IActionResult Schedule()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }

        public IActionResult Marketplace()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }

        public IActionResult MarketplaceChat()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }



        public IActionResult Profile()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }

        public IActionResult SearchLocation()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
            {
                List<LivestockNames> lstock = _context.LivestockType.Select(x => new LivestockNames { Name = x.Name }).ToList();
                return View(lstock);
            }
        }

        public IActionResult SearchResults(string addressString, string livestockType, string rFrom, string rTo)
        {
            _logger.LogInformation(addressString);
            _logger.LogInformation(livestockType);
            _logger.LogInformation(rFrom);
            _logger.LogInformation(rTo);

            ViewData["addressString"] = addressString;
            ViewData["livestockType"] = livestockType;
            ViewData["rFrom"] = rFrom;
            ViewData["rTo"] = rTo;

            if (!IsSubscribed())
                return View("Subscription");
            else {
                List<LivestockNames> lstock = _context.LivestockType.Select(x => new LivestockNames { Name = x.Name }).ToList();
                return View(lstock);
            }
            
        }


        public IActionResult BookingRequest(Guid item)
        {
            ViewData["itemID"] = item;
            if (!IsSubscribed())
                return View("Subscription");
            else
            {
                var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
                var posts = (from mpItem in _context.MarketPlace
                             join user in _context.AspNetUsers
                             on mpItem.User equals user.Id

                             orderby mpItem.DateCreated descending
                             where mpItem.Id == item
                             select new MarketPlaceViewModelRevised
                             {
                                 ItemID = mpItem.Id,
                                 UserId = user.Id,
                                 UserFullname = string.Format("{0} {1}", user.Firstname, user.LastName),
                                 Address = mpItem.Address,
                                 Lat = mpItem.Lat,
                                 Lng = mpItem.Lng,
                                 DateCreated = mpItem.DateCreated.ToString(),
                                 Category = mpItem.Category,
                                 Quantity = (int)mpItem.Quantity,
                                 Total = (decimal)mpItem.Total,
                                 Amount = (decimal)mpItem.Amount,
                                 Kilos = (int)mpItem.Kilos,
                                 ImageList = _context.PostPhoto.Where(p => mpItem.Id == p.Post && p.Path.Contains("marketplace"))
                                                    .Select(p => new MarketPlaceImage { ImageId = p.Id, ImagePath = p.Path })
                                                    .ToList(),
                                 ItemDetails = _context.MarketplaceItemLivestock
                                                    .Where(q => q.MarketplaceItem == mpItem.Id && q.Quantity != 0)
                                                    .Select(q => new MarketPlaceItemDetails
                                                    {
                                                        Id = q.Id,
                                                        MarketplaceItem = q.MarketplaceItem,
                                                        Category = q.Category,
                                                        Unit = q.Unit,
                                                        Quantity = q.Quantity,
                                                        Kilos = q.Kilos,
                                                        Price = q.Price,
                                                        Amount = q.Unit.ToLower().Contains("kilo") ? (q.Kilos * q.Price) * (decimal)q.Quantity : (decimal)q.Quantity * q.Price
                                                    })
                                                    .ToList(),
                                 //Liked = _context.UserLikedPost.Any(q => q.Post == item.Id && q.User == loggedInUser.Id),
                                 IsEditable = loggedInUser.Id == user.Id ? true : false

                             }).FirstOrDefault();
                return View(posts);

            }
        }


        public IActionResult PendingBooking()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
            {
                var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
                var data = (from reqItem in _context.BookingRequestSubItem
                            join req in _context.BookingRequest
                            on reqItem.BookingReqId equals req.Id
                            join farmer in _context.AspNetUsers
                            on req.Farmer equals farmer.Id
                            join subItem in _context.MarketplaceItemLivestock
                            on reqItem.SubItemId equals subItem.Id
                            where req.Vendor == loggedInUser.Id

                            select new FarmerPendingBookingSubItems
                            {
                                Id = reqItem.Id,
                                Category = subItem.Category,
                                Price = subItem.Price,
                                Amount = subItem.Unit.ToLower().Contains("kilo") ? (subItem.Kilos * subItem.Price) * (decimal)reqItem.Quantity : (decimal)reqItem.Quantity * subItem.Price,
                                Kilos = subItem.Kilos,
                                QuantityBooked = reqItem.Quantity,
                                Status = reqItem.Status,
                                Unit = subItem.Unit,
                                Vendor = string.Format("{0} {1}", farmer.Firstname, farmer.LastName),
                                DeliveryDate = reqItem.DeliveryDate,
                                
                            }

                            ).ToList();

                //return View(data);
                return View(data);

            }
        }





        public IActionResult MarkPushNotifAsRead(Guid notifID)
        {
            var notif = _context.Notif.Where(q => q.Id == notifID).FirstOrDefault();
            if(notif != null)
            {
                notif.PushNotifIsRead = "Read";
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IsSubscribed()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
            if (loggedInUser.Subscribed == null || loggedInUser.Subscribed.ToLower() == "unsubscribed")
                return false;
            else
                return true;
        }


    }
}
