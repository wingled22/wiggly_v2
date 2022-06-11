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
using AutoMapper;
using Wiggly.ViewModels;

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
        private readonly IMapper _mapper;

        public TransactionController(WigglyContext context, UserManager<AppUser> usrMngr, SignInManager<AppUser> signInMngr, IMapper mapper)
        {
            _context = context;
            _usrMngr = usrMngr;
            _signInMngr = signInMngr;
            _mapper = mapper;
        }

        // GET: api/Transaction
        [HttpGet]
        public ActionResult GetTransactions()
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();

            var transactions = (from trnsctn in _context.Transaction
                                join frmer in _context.AspNetUsers on trnsctn.Farmer equals frmer.Id
                                where trnsctn.Vendor == loggedInUser.Id

                                select new TransactionInfoViewModel
                                {
                                    TransactionID = trnsctn.Id,
                                    Farmer = (int)trnsctn.Farmer,
                                    FarmerFullname = frmer.Firstname + " " + frmer.LastName,
                                    BookDate = trnsctn.BookDate,
                                    LiveStockType = trnsctn.TypeOfLivestock,
                                    Kilos = trnsctn.Kilos,
                                    Quantity = (int)trnsctn.Quantity,
                                    PaymentType = trnsctn.PaymentType,
                                    Status = trnsctn.PaymentStatus,
                                    Amount = trnsctn.Amount,
                                    ProofOfpayment = trnsctn.ProofOfPayment
                                }).ToList();

            //var transactions = _context.Transaction.Where(q => q.Vendor == loggedInUser.Id).ToList();

            return Ok(transactions);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(string values, string proofPayment)
        {
            var loggedInUser = _context.AspNetUsers.Where(q => q.UserName == this.User.Identity.Name).FirstOrDefault();


            var info = new TransactionInfoViewModel();

            JsonConvert.PopulateObject(values, info);

            if (!TryValidateModel(info))
                return BadRequest("Values are incorrect");


            Transaction transaction = new Transaction {
                Vendor = loggedInUser.Id,
                Farmer = info.Farmer,
                //BookDate = info.BookDate,
                DateCreated = DateTime.Now,
                Status = "Pending"
            };

            if (!TryValidateModel(transaction))
                return BadRequest("Transaction info are incorrect");

            _context.Transaction.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok();

        }


        [HttpPut]
        public async Task<IActionResult> PutTransaction( string values, string proofPayment)
        {

            var info = new TransactionInfoViewModel();

            JsonConvert.PopulateObject(values, info);

            var transaction = _context.Transaction.Where(q => q.Id == info.TransactionID).FirstOrDefault();

            if (transaction == null )
            {
                return BadRequest("Requested data not found!");
            }

            if(info.PaymentType == "Cash on Hand")
            {
                transaction.PaymentStatus = "Pending";
                transaction.PaymentType = info.PaymentType;
                
            }
            else
            {
                transaction.PaymentStatus = "Pending";
                transaction.PaymentType = info.PaymentType;
                transaction.ProofOfPayment = proofPayment.ToString();
            }


            if (!TryValidateModel(transaction))
                return BadRequest("transaction infos are incorrect");
     

            _context.Transaction.Update(transaction);
            await _context.SaveChangesAsync();

            return Ok();
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

        [HttpGet]
        public ActionResult GetFarmersWithFullname()
        {
            var res = _context.AspNetUsers.Where(q => q.UserType == "Farmer").ToList();
            if (res == null)
                NoContent();

            //var result = _mapper.Map<List<AspNetUsers>, List<UsersWithFullname>>(res);
            var result = _mapper.Map<List<UsersWithFullname>>(res);
            return Ok(result);
        }

        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.Id == id);
        }
    }
}
