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
                Type = info.PaymentType,
                Status = "Pending",
                Transaction = transaction.Id,
                Vendor = loggedInUser.Id,
                Farmer = info.Farmer,
                Amount = info.Amount,
                Total = info.Amount
            };

            _context.Payment.Add(payment);
            await _context.SaveChangesAsync();


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
            var info = (from trnsctn in _context.Transaction
                        join kg in _context.Kilos on trnsctn.Id equals kg.Transaction
                        join pymt in _context.Payment on trnsctn.Id equals pymt.Transaction
                        where trnsctn.Id == key

                        select new TransactionInfoViewModel
                        {
                            TransactionID = trnsctn.Id,
                            Farmer = (int)trnsctn.Farmer,
                            BookDate = trnsctn.BookDate,
                            PorkNum = kg.PorkNum,
                            Pork = kg.Pork,
                            BeefNum = kg.BeefNum,
                            Beef = kg.Beef,
                            ChickenNum = kg.ChickenNum,
                            Chicken = kg.Chicken,
                            GoatNum = kg.GoatNum,
                            Goat = kg.Goat,
                            CarabaoNum = kg.CarabaoNum,
                            Carabao = kg.Carabao,
                            PaymentType = pymt.Type,
                            Amount = pymt.Amount
                        }).FirstOrDefault(); 



            JsonConvert.PopulateObject(values, info);

            var transaction = _context.Transaction.Where(q => q.Id == key).FirstOrDefault();
            var kilo = _context.Kilos.Where(q => q.Transaction == key).FirstOrDefault();
            var payment = _context.Payment.Where(q => q.Transaction == key).FirstOrDefault();

            if (transaction == null || kilo == null || payment == null)
            {
                return BadRequest("Requested data not found!");
            }

            //transaction
            if (!String.IsNullOrEmpty(info.Farmer.ToString()))
                transaction.Farmer = info.Farmer;
            if (info.BookDate != null)
                transaction.BookDate = info.BookDate;

            //kilos
            if (info.PorkNum != null)
                kilo.PorkNum = info.PorkNum;
            if (info.Pork != null)
                kilo.Pork = info.Pork;
            if (info.BeefNum != null)
                kilo.BeefNum = info.BeefNum;
            if (info.Beef != null)
                kilo.Beef = info.Beef;
            if (info.ChickenNum != null)
                kilo.ChickenNum = info.ChickenNum;
            if (info.Chicken != null)
                kilo.Chicken = info.Chicken;
            if (info.GoatNum != null)
                kilo.GoatNum = info.GoatNum;
            if (info.Goat != null)
                kilo.Goat = info.Goat;
            if (info.CarabaoNum != null)
                kilo.CarabaoNum = info.CarabaoNum;
            if (info.Carabao != null)
                kilo.Carabao = info.Carabao;

            //payment
            if (info.PaymentType != null)
                payment.Type = info.PaymentType;
            if (info.Amount != null)
                payment.Amount = info.Amount; payment.Total = info.Amount;



            //JsonConvert.PopulateObject(values, transaction);

            if (!TryValidateModel(transaction))
                return BadRequest("transaction infos are incorrect");
            if (!TryValidateModel(kilo))
                return BadRequest("kilos infos are incorrect");
            if (!TryValidateModel(payment))
                return BadRequest("payment infos are incorrect");

            _context.Transaction.Update(transaction);
            await _context.SaveChangesAsync();

            _context.Kilos.Update(kilo);
            await _context.SaveChangesAsync();

            _context.Payment.Update(payment);
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
