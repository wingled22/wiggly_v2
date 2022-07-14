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


        public IActionResult SearchResultsPutangIna(string addressString, string livestockType, string rFrom, string rTo)
        {
            _logger.LogInformation(addressString);
            _logger.LogInformation(livestockType);

          

            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            List<MarketPlaceViewModelRevised> result;


            List<SubItemID> listOfId = new List<SubItemID>();

            if (string.IsNullOrEmpty(addressString))
                _logger.LogInformation("address is empty");
            else
                _logger.LogInformation("address is not empty");


            decimal? rangeFrom = null;
            decimal? rangeTo = null;

            if (!string.IsNullOrEmpty(rFrom))
                rangeFrom = decimal.Parse(rFrom);

            if (!string.IsNullOrEmpty(rTo))
                rangeTo = decimal.Parse(rTo);




            //search by pricelist and livestock type
            if (rangeFrom == null && rangeTo == null)
            {
                _logger.LogInformation("price ranges are null");
                List<SubItemID> resultList = _context.MarketplaceItemLivestock
                                            .Where(q => q.Category == livestockType)
                                            .Select(s => new SubItemID { Item = (Guid)s.MarketplaceItem })
                                            .Distinct()
                                            .ToList();
                listOfId.AddRange(resultList);
            }
            else if (rangeFrom != null && rangeTo == null)
            {
                _logger.LogInformation("price rangefrom is not null");
                List<SubItemID> resultList = _context.MarketplaceItemLivestock
                                                .Where(q => q.Category == livestockType && q.Price >= rangeFrom )
                                                .Select(s => new SubItemID { Item = (Guid)s.MarketplaceItem })
                                                .Distinct()
                                                .ToList();
                listOfId.AddRange(resultList);
            }
            else if (rangeFrom == null && rangeTo != null)
            {
                _logger.LogInformation("price rangeto is not null");

                List<SubItemID> resultList = _context.MarketplaceItemLivestock
                                                .Where(q => q.Category == livestockType && rangeTo >= q.Price)
                                                .Select(s => new SubItemID { Item = (Guid)s.MarketplaceItem })
                                                .Distinct()
                                                .ToList();
                listOfId.AddRange(resultList);
            }
            else if (rangeFrom != null && rangeTo != null)
            {
                _logger.LogInformation("price rangefrom are not null");
                List<SubItemID> resultList = _context.MarketplaceItemLivestock
                                                .Where(q => q.Category == livestockType && rangeFrom <= q.Price && rangeTo >= q.Price)
                                                .Select(s => new SubItemID { Item = (Guid)s.MarketplaceItem })
                                                .Distinct()
                                                .ToList();

                listOfId.AddRange(resultList);
            }

            //_logger.LogInformation("length of match subitem : " + listOfId.Count);

            //get the marketplace item
            List<MarketPlaceViewModelRevised> ItemsList = new List<MarketPlaceViewModelRevised>();
            if (string.IsNullOrEmpty(addressString))
            {
                foreach (var item in listOfId)
                {
                    var post = (from mp in _context.MarketPlace
                                join user in _context.AspNetUsers
                                on mp.User equals user.Id

                                orderby mp.DateCreated descending
                                where mp.Id == item.Item
                                select new MarketPlaceViewModelRevised
                                {
                                    ItemID = mp.Id,
                                    UserId = user.Id,
                                    UserFullname = string.Format("{0} {1}", user.Firstname, user.LastName),
                                    Address = mp.Address,
                                    Lat = mp.Lat,
                                    Lng = mp.Lng,
                                    DateCreated = mp.DateCreated.ToString(),
                                    Category = mp.Category,
                                    Quantity = (int)mp.Quantity,
                                    Total = (decimal)mp.Total,
                                    Amount = (decimal)mp.Amount,
                                    Kilos = (int)mp.Kilos,
                                    ImageList = _context.PostPhoto.Where(p => mp.Id == p.Post && p.Path.Contains("marketplace"))
                                                       .Select(p => new MarketPlaceImage { ImageId = p.Id, ImagePath = p.Path })
                                                       .ToList(),
                                    ItemDetails = _context.MarketplaceItemLivestock
                                                       .Where(q => q.MarketplaceItem == mp.Id && q.Quantity != 0)
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
                                    IsEditable = loggedInUser.Id == user.Id ? true : false,
                                    Status = _context.BookingRequest.Any(q => q.Farmer == mp.User && q.Vendor == loggedInUser.Id && q.Item == mp.Id && q.Status == "Pending") ? "Pending" : "Not booked"

                                }).FirstOrDefault();
                    if(post != null)
                        ItemsList.Add(post);
                }
            }
            else
            {
                foreach (var item in listOfId)
                {
                    var post = (from mp in _context.MarketPlace
                                join user in _context.AspNetUsers
                                on mp.User equals user.Id

                                orderby mp.DateCreated descending
                                where mp.Id == item.Item && EF.Functions.Like(mp.Address, string.Format("%{0}%", addressString))
                                select new MarketPlaceViewModelRevised
                                {
                                    ItemID = mp.Id,
                                    UserId = user.Id,
                                    UserFullname = string.Format("{0} {1}", user.Firstname, user.LastName),
                                    Address = mp.Address,
                                    Lat = mp.Lat,
                                    Lng = mp.Lng,
                                    DateCreated = mp.DateCreated.ToString(),
                                    Category = mp.Category,
                                    Quantity = (int)mp.Quantity,
                                    Total = (decimal)mp.Total,
                                    Amount = (decimal)mp.Amount,
                                    Kilos = (int)mp.Kilos,
                                    ImageList = _context.PostPhoto.Where(p => mp.Id == p.Post && p.Path.Contains("marketplace"))
                                                       .Select(p => new MarketPlaceImage { ImageId = p.Id, ImagePath = p.Path })
                                                       .ToList(),
                                    ItemDetails = _context.MarketplaceItemLivestock
                                                       .Where(q => q.MarketplaceItem == mp.Id && q.Quantity != 0)
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
                                    IsEditable = loggedInUser.Id == user.Id ? true : false,
                                    Status = _context.BookingRequest.Any(q => q.Farmer == mp.User && q.Vendor == loggedInUser.Id && q.Item == mp.Id && q.Status == "Pending") ? "Pending" : "Not booked"

                                }).FirstOrDefault();

                    if(post != null)
                        ItemsList.Add(post);

                }
            }


            return Ok(ItemsList);
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
                minRange = 1000;
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
    class SubItemID
    {
        public Guid Item { get; set; }
    }
}
