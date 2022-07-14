using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Wiggly.Entities;

namespace Wiggly.Areas_Admin_Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class UnitOfMeasureController : Controller
    {
        private readonly WigglyContext _context;

        public UnitOfMeasureController(WigglyContext context)
        {
            _context = context;
        }

        // GET: UnitOfMeasure
        public async Task<IActionResult> Index()
        {
            return View(await _context.UnitOfMeasure.ToListAsync());
        }

        // GET: UnitOfMeasure/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasure = await _context.UnitOfMeasure
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unitOfMeasure == null)
            {
                return NotFound();
            }

            return View(unitOfMeasure);
        }

        // GET: UnitOfMeasure/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UnitOfMeasure/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] UnitOfMeasure unitOfMeasure)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unitOfMeasure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(unitOfMeasure);
        }

        // GET: UnitOfMeasure/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasure = await _context.UnitOfMeasure.FindAsync(id);
            if (unitOfMeasure == null)
            {
                return NotFound();
            }
            return View(unitOfMeasure);
        }

        // POST: UnitOfMeasure/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] UnitOfMeasure unitOfMeasure)
        {
            if (id != unitOfMeasure.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var old = _context.UnitOfMeasure.Where(q => q.Id == id).AsNoTracking().FirstOrDefault();

                    var mpItems = _context.MarketPlace.Where(q => q.Category == old.Name).ToList();
                    var transactions = _context.Transaction.Where(q => q.TypeOfLivestock == old.Name).ToList();
                    var mpSubItems = _context.MarketplaceItemLivestock.Where(q => q.Category == old.Name).ToList();
                    var transactionSubItems = _context.TransactionSubItem.Where(q => q.Category == old.Name).ToList();

                    //if (mpItems.Count() > 0)
                    //{
                    //    foreach (var item in mpItems)
                    //    {
                    //        item. = unitOfMeasure.Name;
                    //    }
                    //}

                    //if (transactions.Count() > 0)
                    //{
                    //    foreach (var item in transactions)
                    //    {
                    //        item.uni = unitOfMeasure.Name;
                    //    }
                    //}

                    if (mpSubItems.Count() > 0)
                    {
                        foreach (var item in mpSubItems)
                        {
                            item.Unit = unitOfMeasure.Name;
                        }
                    }

                    if (transactionSubItems.Count() > 0)
                    {
                        foreach (var item in transactionSubItems)
                        {
                            item.Units = unitOfMeasure.Name;
                        }
                    }

                    _context.Update(unitOfMeasure);
                    await _context.SaveChangesAsync();

                    //_context.MarketPlace.UpdateRange(mpItems);
                    //await _context.SaveChangesAsync();

                    //_context.Transaction.UpdateRange(transactions);
                    //await _context.SaveChangesAsync();

                    _context.MarketplaceItemLivestock.UpdateRange(mpSubItems);
                    await _context.SaveChangesAsync();

                    _context.TransactionSubItem.UpdateRange(transactionSubItems);
                    await _context.SaveChangesAsync();


                    //_context.Update(unitOfMeasure);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitOfMeasureExists(unitOfMeasure.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(unitOfMeasure);
        }

        // GET: UnitOfMeasure/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasure = await _context.UnitOfMeasure
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unitOfMeasure == null)
            {
                return NotFound();
            }

            return View(unitOfMeasure);
        }

        // POST: UnitOfMeasure/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unitOfMeasure = await _context.UnitOfMeasure.FindAsync(id);
            _context.UnitOfMeasure.Remove(unitOfMeasure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UnitOfMeasureExists(int id)
        {
            return _context.UnitOfMeasure.Any(e => e.Id == id);
        }
    }
}
