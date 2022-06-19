using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Wiggly.Entities;

namespace Wiggly.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class LiveStockTypesController : Controller
    {
        private readonly WigglyContext _context;

        public LiveStockTypesController(WigglyContext context)
        {
            _context = context;
        }

        // GET: LiveStockTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.LivestockType.ToListAsync());
        }

        // GET: LiveStockTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livestockType = await _context.LivestockType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livestockType == null)
            {
                return NotFound();
            }

            return View(livestockType);
        }

        // GET: LiveStockTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LiveStockTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LivestockType livestockType)
        {
            if (ModelState.IsValid)
            {
                livestockType.DateCreated = DateTime.Now;
                _context.Add(livestockType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(livestockType);
        }

        // GET: LiveStockTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livestockType = await _context.LivestockType.FindAsync(id);
            if (livestockType == null)
            {
                return NotFound();
            }
            return View(livestockType);
        }

        // POST: LiveStockTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LivestockType livestockType)
        {
            if (id != livestockType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var old = _context.LivestockType.Where(q => q.Id == id).AsNoTracking().FirstOrDefault();

                    var mpItems = _context.MarketPlace.Where(q => q.Category == old.Name).ToList();
                    var transactions = _context.Transaction.Where(q => q.TypeOfLivestock == old.Name).ToList();

                   
                    if(mpItems.Count() > 0)
                    {
                        foreach (var item in mpItems)
                        {
                            item.Category = livestockType.Name;
                        }
                    }

                    if(transactions.Count() > 0)
                    {
                        foreach (var item in transactions)
                        {
                            item.TypeOfLivestock = livestockType.Name;
                        }
                    }

                    _context.Update(livestockType);
                    await _context.SaveChangesAsync();

                    _context.MarketPlace.UpdateRange(mpItems);
                    await _context.SaveChangesAsync();

                    _context.Transaction.UpdateRange(transactions);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivestockTypeExists(livestockType.Id))
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
            return View(livestockType);
        }

        // GET: LiveStockTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livestockType = await _context.LivestockType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livestockType == null)
            {
                return NotFound();
            }

            return View(livestockType);
        }

        // POST: LiveStockTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livestockType = await _context.LivestockType.FindAsync(id);
            _context.LivestockType.Remove(livestockType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LivestockTypeExists(int id)
        {
            return _context.LivestockType.Any(e => e.Id == id);
        }
    }
}
