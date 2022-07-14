using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Controllers;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Areas.Farmer.Controllers
{
    [Authorize(Roles = "Farmer")]
    [Area("Farmer")]
    public class SubItemAPIController : Controller
    {

        private readonly ILogger<SubItemAPIController> _logger;
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;
        public SubItemAPIController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, ILogger<SubItemAPIController> logger)
        {
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _context = context;
            _logger = logger;
        }


        public ActionResult GetSubItems(Guid postId)
        {
            var subItems = _context.MarketplaceItemLivestock.Where(q => q.MarketplaceItem == postId)
                            .Select(q => new 
                                MarketPlaceItemDetails
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
                                .ToList();  

            if (subItems != null)
                return Ok(subItems);
            return Ok();
        }

        public async Task<ActionResult<MarketplaceItemLivestock>> PostSubItems(string values)
        {

            if (!string.IsNullOrEmpty(values))
            {
                var article = new MarketplaceItemLivestock();
                JsonConvert.PopulateObject(values, article);

                if (!TryValidateModel(article))
                    return BadRequest("Validation error");

                _context.MarketplaceItemLivestock.Add(article);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("No data sent!");
            }

        }

        public async Task<IActionResult> PutSubItems(long key, string values)
        {

            if (!string.IsNullOrEmpty(values))
            {
                var selected = _context.MarketplaceItemLivestock.Where(q => q.Id == key).FirstOrDefault();
                MarketPlaceItemDetails newval = new MarketPlaceItemDetails();

                JsonConvert.PopulateObject(values, newval);

                if(!string.IsNullOrEmpty(newval.Category))
                    selected.Category = newval.Category;
                
                if(!string.IsNullOrEmpty(newval.Unit))
                    selected.Unit = newval.Unit;
                
                if(newval.Quantity != null)
                    selected.Quantity= newval.Quantity;
                
                if(newval.Kilos != null)
                    selected.Kilos = newval.Kilos;
                
                if(newval.Price != null)
                    selected.Price = newval.Price;

                if (!TryValidateModel(selected))
                    return BadRequest("Validation error");

                _context.MarketplaceItemLivestock.Update(selected);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return BadRequest("No data sent!");
            }

        }
    }
}
