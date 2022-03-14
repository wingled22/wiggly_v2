using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wiggly.Entities;
using Newtonsoft.Json;
using Wiggly.Areas.Vendor.Models;
using Microsoft.AspNetCore.Identity;
using Wiggly.Identity;

namespace Wiggly.Areas.Vendor.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    [Area("Vendor")]
    public class TransactionController : Controller
    {
        private UserManager<AppUser> _usrMngr;
        private SignInManager<AppUser> _signInMngr;
        private readonly WigglyContext _context;

        public TransactionController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr)
        {
            _context = context;
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
        }

        // GET: api/Transaction
        [HttpGet]
        public ActionResult GetTransactions()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var transactions = (from trnsctn in _context.Transaction
                                join kilos in _context.Kilos on trnsctn.Id equals kilos.Transaction
                                join pymt in _context.Payment on trnsctn.Id equals pymt.Transaction
                                where trnsctn.Vendor == loggedInUser.Id

                                select new TransactionInfoViewModel
                                {
                                    TransactionID = trnsctn.Id,
                                    Farmer = (int)trnsctn.Farmer,
                                    BookDate = trnsctn.BookDate,
                                    PorkNum = kilos.PorkNum,
                                    Pork = kilos.Pork,
                                    BeefNum = kilos.BeefNum,
                                    Beef = kilos.Beef,
                                    ChickenNum = kilos.ChickenNum,
                                    Chicken = kilos.Chicken,
                                    GoatNum = kilos.GoatNum,
                                    Goat = kilos.Goat,
                                    CarabaoNum = kilos.CarabaoNum,
                                    Carabao = kilos.Carabao,
                                    PaymentType = pymt.Type,
                                    Amount = pymt.Amount
                                }).ToList();

            return Ok(transactions);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(string values)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();


            var info = new TransactionInfoViewModel();
            JsonConvert.PopulateObject(values, info);

            if (!TryValidateModel(info))
                return BadRequest("Values are incorrect");


            Transaction transaction = new Transaction {
                Vendor = loggedInUser.Id,
                Farmer = info.Farmer,
                BookDate = info.BookDate,
                DateCreated = DateTime.Now
            };

            if (!TryValidateModel(transaction))
                return BadRequest("Transaction info are incorrect");

            _context.Transaction.Add(transaction);
            await _context.SaveChangesAsync();

            Kilos kilos = new Kilos { 
                Transaction = transaction.Id,
                PorkNum = info.PorkNum,
                Pork = info.Pork,
                BeefNum = info.BeefNum,
                Beef = info.Beef,
                ChickenNum = info.ChickenNum,
                Chicken = info.Chicken,
                GoatNum = info.GoatNum,
                Goat = info.Goat,
                CarabaoNum = info.CarabaoNum,
                Carabao = info.Carabao

            };
            _context.Kilos.Add(kilos);
            await _context.SaveChangesAsync();

            Payment payment = new Payment { 
            
            };




            //if (!TryValidateModel(transaction))
            //    return BadRequest("Values are incorrect");

            //_context.Transaction.Add(transaction);
            //await _context.SaveChangesAsync();
            return Ok();

        }


        [HttpPut]
        public async Task<IActionResult> PutTransaction(int? key, string values)
        {
            if (key == null)
            {
                return BadRequest("Data sent is incomplete!");
            }

            var transaction = _context.Transaction.Where(q => q.Id == key).FirstOrDefault();

            if (transaction == null)
            {
                return BadRequest("Requested data not found!");
            }

            JsonConvert.PopulateObject(values, transaction);

            if (!TryValidateModel(transaction))
                return BadRequest("values are incorrect");

            _context.Transaction.Update(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        
        [HttpDelete]
        public async Task<ActionResult<Transaction>> DeleteTransaction(int? key)
        {
            if (key == null)
            {
                return BadRequest("Id has no value");
            }

            var transaction = await _context.Transaction.FindAsync(key);
            
            if (transaction == null)
            {
                return BadRequest("Book not found");
            }
            
            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        [HttpGet]
        public ActionResult GetFarmers()
        {
            var res = _context.AspNetUsers.Where(q => q.UserType == "Farmer").ToList();
            if (res == null)
                NoContent();
            return Ok(res);
        }

        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.Id == id);
        }
    }
}
