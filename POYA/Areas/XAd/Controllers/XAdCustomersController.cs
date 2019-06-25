using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.XAd.Models;
using POYA.Data;

namespace POYA.Areas.XAd.Controllers
{
    [Area("XAd")]
    public class XAdCustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public XAdCustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: XAd/XAdCustomers
        public async Task<IActionResult> Index()
        {
            return View(await _context.XAdCustomer.ToListAsync());
        }

        // GET: XAd/XAdCustomers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xAdCustomer = await _context.XAdCustomer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xAdCustomer == null)
            {
                return NotFound();
            }

            return View(xAdCustomer);
        }

        // GET: XAd/XAdCustomers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: XAd/XAdCustomers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,UserId,DORegistering,Address")] XAdCustomer xAdCustomer)
        {
            if (ModelState.IsValid)
            {
                xAdCustomer.Id = Guid.NewGuid();
                _context.Add(xAdCustomer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(xAdCustomer);
        }

        // GET: XAd/XAdCustomers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xAdCustomer = await _context.XAdCustomer.FindAsync(id);
            if (xAdCustomer == null)
            {
                return NotFound();
            }
            return View(xAdCustomer);
        }

        // POST: XAd/XAdCustomers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,UserId,DORegistering,Address")] XAdCustomer xAdCustomer)
        {
            if (id != xAdCustomer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xAdCustomer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XAdCustomerExists(xAdCustomer.Id))
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
            return View(xAdCustomer);
        }

        // GET: XAd/XAdCustomers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xAdCustomer = await _context.XAdCustomer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xAdCustomer == null)
            {
                return NotFound();
            }

            return View(xAdCustomer);
        }

        // POST: XAd/XAdCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var xAdCustomer = await _context.XAdCustomer.FindAsync(id);
            _context.XAdCustomer.Remove(xAdCustomer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool XAdCustomerExists(Guid id)
        {
            return _context.XAdCustomer.Any(e => e.Id == id);
        }
    }
}
