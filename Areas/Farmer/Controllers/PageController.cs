using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Controllers;
using Wiggly.Entities;
using Wiggly.Identity;
using Wiggly.Models;

namespace Wiggly.Areas.Farmer.Controllers
{
    [Authorize(Roles = "Farmer")]
    [Area("Farmer")]
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

        public IActionResult EditPost(Guid Id)
        {
            ViewData["ItemId"] = Id;
            if (!IsSubscribed())
                return View("Subscription");
            else
            {
                var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();
                var posts = (from item in _context.MarketPlace
                             join user in _context.AspNetUsers
                             on item.User equals user.Id

                             orderby item.DateCreated descending
                             where item.Id == Id
                             select new MarketPlaceViewModelRevised
                             {
                                 ItemID = item.Id,
                                 UserId = user.Id,
                                 UserFullname = string.Format("{0} {1}", user.Firstname, user.LastName),
                                 Address = item.Address,
                                 Lat = item.Lat,
                                 Lng = item.Lng,
                                 DateCreated = item.DateCreated.ToString(),
                                 Category = item.Category,
                                 Quantity = (int)item.Quantity,
                                 Total = (decimal)item.Total,
                                 Amount = (decimal)item.Amount,
                                 Kilos = (int)item.Kilos,
                                 ImageList = _context.PostPhoto.Where(p => item.Id == p.Post && p.Path.Contains("marketplace"))
                                                    .Select(p => new MarketPlaceImage { ImageId = p.Id, ImagePath = p.Path })
                                                    .ToList(),
                                 ItemDetails = _context.MarketplaceItemLivestock
                                                    .Where(q => q.MarketplaceItem == item.Id && q.Quantity != 0)
                                                    .Select(q => new MarketPlaceItemDetails
                                                    {
                                                        Id = q.Id,
                                                        MarketplaceItem = q.MarketplaceItem,
                                                        Category = q.Category,
                                                        Unit = q.Unit,
                                                        Quantity = q.Quantity,
                                                        Kilos = q.Kilos,
                                                        Price = q.Price,
                                                        Amount = (decimal)q.Quantity * q.Price
                                                    })
                                                    .ToList(),
                                 //Liked = _context.UserLikedPost.Any(q => q.Post == item.Id && q.User == loggedInUser.Id),
                                 IsEditable = loggedInUser.Id == user.Id ? true : false

                             }).FirstOrDefault();
                return View(posts);
            }
        }

        public IActionResult Calendar()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View("Calendar-v2");
        }

        public IActionResult Transaction()
        {
            if (!IsSubscribed())
                return View("Subscription");
            else
                return View();
        }

        public IActionResult Chat()
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
                return View();
        }

        public IActionResult SearchResults(string addressString, string livestockType, string priceRange)
        {
            
            _logger.LogInformation(addressString);

            ViewData["addressString"] = addressString;



            if (!IsSubscribed())
                return View("Subscription");
            else if (string.IsNullOrEmpty(addressString))
                return View("SearchLocation");
            else
            {
                var vendors = _context.AspNetUsers.Where(q => EF.Functions.Like(q.Address, string.Format("%{0}%", addressString))).ToList();
                return View(vendors);
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
                            join vendor in _context.AspNetUsers
                            on req.Vendor equals vendor.Id
                            join subItem in _context.MarketplaceItemLivestock
                            on reqItem.SubItemId equals subItem.Id
                            where reqItem.Status == "Pending" && req.Farmer == loggedInUser.Id

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
                                Vendor = string.Format("{0} {1}", vendor.Firstname, vendor.LastName),
                                DeliveryDate = reqItem.DeliveryDate
                            }

                            ).ToList();

                return View(data);


            }
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
