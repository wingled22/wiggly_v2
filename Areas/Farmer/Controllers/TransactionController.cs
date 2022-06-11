using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wiggly.Areas.Farmer.Models;
using Wiggly.Entities;
using Wiggly.Identity;

namespace Wiggly.Areas.Farmer.Controllers
{
    [Area("Farmer")]
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


        [HttpGet]
        public ActionResult GetTransactions()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var transactions = (from trnsctn in _context.Transaction
                                join frmer in _context.AspNetUsers on trnsctn.Vendor equals frmer.Id
                                where trnsctn.Farmer == loggedInUser.Id && trnsctn.PaymentType != null 

                                select new TransactionInfoViewModel
                                {
                                    TransactionID = trnsctn.Id,
                                    Farmer = (int)trnsctn.Farmer,
                                    Vendor = (int)trnsctn.Vendor,
                                    VendorFullname = frmer.Firstname+" "+frmer.LastName,
                                    BookDate = trnsctn.BookDate,
                                    LiveStockType = trnsctn.TypeOfLivestock,
                                    Kilos = trnsctn.Kilos,
                                    Quantity = (int)trnsctn.Quantity,
                                    PaymentType = trnsctn.PaymentType,
                                    Status = trnsctn.PaymentStatus,
                                    Amount = trnsctn.Amount,
                                    ProofOfpayment = trnsctn.ProofOfPayment
                                }).ToList();

            return Ok(transactions);
        }

        [HttpPut]
        public async Task<IActionResult> PutTransaction(int? key, string values)
        {
            if (key == null)
            {
                return BadRequest("Data sent is incomplete!");

            }


            var info = new TransactionInfoViewModel();
            JsonConvert.PopulateObject(values, info);



            Transaction transaction = _context.Transaction.Where(q => q.Id == key).FirstOrDefault();
            transaction.PaymentStatus = info.Status;

            _context.Transaction.Update(transaction);
            await _context.SaveChangesAsync();

            //Payment payment = _context.Payment.Where(q => q.Transaction == key).FirstOrDefault();
            //payment.Status = info.Status;
            //_context.Payment.Update(payment);
            //await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public ActionResult GetVendors()
        {
            var res = _context.AspNetUsers.Where(q => q.UserType == "Farmer").ToList();
            if (res == null)
                NoContent();
            return Ok(res);
        }
    }
}
