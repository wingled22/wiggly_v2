using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class SearchAPIController : Controller
    {

        private readonly ILogger<SearchAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public SearchAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<SearchAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }

        public IActionResult SearchResults(string addressString, string livestockType, string priceRange)
        {
            _logger.LogInformation(addressString);
            _logger.LogInformation(livestockType);
            _logger.LogInformation(priceRange);

            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            List<MarketPlaceViewModelRevised>    result;
            decimal minRange = 0, maxRange = 0;
            if(priceRange == "option1")
            {
                minRange = 5000;
                maxRange = 15000;
            }else if (priceRange == "option2")
            {
                minRange = 15000;
                maxRange = 25000;
            }

            if (string.IsNullOrEmpty(addressString))
            {
                if(priceRange == "option3")
                {
                    _logger.LogInformation("is option3 above 25000");
                    //res =_context.MarketPlace.Where(q => q.Category.ToLower() == livestockType && q.Amount > 25000).ToList();

                    result = (from item in _context.MarketPlace
                                  join user in _context.AspNetUsers
                                  on item.User equals user.Id
                              where item.Category == livestockType && 25000 <= item.Amount
                              orderby item.DateCreated descending

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
                                      Amount = (decimal)item.Amount,
                                      Kilos = (int)item.Kilos,
                                      ImageList = _context.PostPhoto.Where(p => item.Id == p.Post).Select(p => new MarketPlaceImage { ImageId = p.Id, ImagePath = p.Path }).ToList(),
                                      Status = _context.BookingRequest.Any(q => q.Farmer == item.User && q.Vendor == loggedInUser.Id && q.Item == item.Id && q.Status == "Pending") ? "Pending" : "Not booked"
                                  }).ToList();
                    return Ok(result);
                }
                else
                {
                    result = (from item in _context.MarketPlace
                              join user in _context.AspNetUsers
                              on item.User equals user.Id
                              where item.Category == livestockType && minRange <= item.Amount &&  maxRange >= item.Amount
                              orderby item.DateCreated descending

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
                                  Amount = (decimal)item.Amount,
                                  Kilos = (int)item.Kilos,
                                  ImageList = _context.PostPhoto.Where(p => item.Id == p.Post).Select(p => new MarketPlaceImage { ImageId = p.Id, ImagePath = p.Path }).ToList(),
                                  Status = _context.BookingRequest.Any(q => q.Farmer == item.User && q.Vendor == loggedInUser.Id && q.Item == item.Id && q.Status == "Pending") ? "Pending" : "Not booked"
                              }).ToList();
                    return Ok(result);
                }
            
            }
            else
            {
                _logger.LogInformation(addressString);

                if (priceRange == "option3")
                {
                    _logger.LogInformation("is option3 above 25000");
                    //res =_context.MarketPlace.Where(q => q.Category.ToLower() == livestockType && q.Amount > 25000).ToList();

                    result = (from item in _context.MarketPlace
                              join user in _context.AspNetUsers
                              on item.User equals user.Id
                              where  item.Category == livestockType &&  25000 <= item.Amount && EF.Functions.Like(item.Address, string.Format("%{0}%", addressString))

                              orderby item.DateCreated descending

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
                                  Amount = (decimal)item.Amount,
                                  Kilos = (int)item.Kilos,
                                  ImageList = _context.PostPhoto.Where(p => item.Id == p.Post).Select(p => new MarketPlaceImage { ImageId = p.Id, ImagePath = p.Path }).ToList(),
                                  Status = _context.BookingRequest.Any(q => q.Farmer == item.User && q.Vendor == loggedInUser.Id && q.Item == item.Id && q.Status == "Pending") ? "Pending" : "Not booked"
                              }).ToList();
                    return Ok(result);
                }
                else
                {
                    result = (from item in _context.MarketPlace
                              join user in _context.AspNetUsers
                              on item.User equals user.Id
                              where item.Category == livestockType && minRange <= item.Amount && maxRange >= item.Amount && EF.Functions.Like(item.Address, string.Format("%{0}%", addressString))
                              orderby item.DateCreated descending

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
                                  Amount = (decimal)item.Amount,
                                  Kilos = (int)item.Kilos,
                                  ImageList = _context.PostPhoto.Where(p => item.Id == p.Post).Select(p => new MarketPlaceImage { ImageId = p.Id, ImagePath = p.Path }).ToList(),
                                  Status = _context.BookingRequest.Any(q => q.Farmer == item.User && q.Vendor == loggedInUser.Id && q.Item==item.Id && q.Status == "Pending") ? "Pending" : "Not booked"
                              }).ToList();
                    //var res = _context.MarketPlace.Where(q => EF.Functions.Like(q.Address, string.Format("%{0}%",addressString))).ToList();
                    return Ok(result);
                }

            }

            return Ok();
        }
    
        public IActionResult SearchVendorByLocation(string addressString)
        {
            return Ok();
        }
    }
}
