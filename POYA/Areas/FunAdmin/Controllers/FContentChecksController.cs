using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.FunAdmin.Models;
using POYA.Data;

namespace POYA.Areas.FunAdmin.Controllers
{
    [Area("FunAdmin")]
    public class FContentChecksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FContentChecksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FunAdmin/FContentChecks
        public async Task<IActionResult> Index()
        {
            return View(await _context.FContentCheck.ToListAsync());
        }

        // GET: FunAdmin/FContentChecks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fContentCheck = await _context.FContentCheck
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fContentCheck == null)
            {
                return NotFound();
            }

            return View(fContentCheck);
        }

        // GET: FunAdmin/FContentChecks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FunAdmin/FContentChecks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContentId,AppellantId,ReceptionistId,DOSubmitting,DOHandling,AppellantContent,ReceptionistContent,IsLegal,ContentTitle")] FContentCheck fContentCheck)
        {
            if (ModelState.IsValid)
            {
                fContentCheck.Id = Guid.NewGuid();
                _context.Add(fContentCheck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fContentCheck);
        }

        // GET: FunAdmin/FContentChecks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fContentCheck = await _context.FContentCheck.FindAsync(id);
            if (fContentCheck == null)
            {
                return NotFound();
            }
            return View(fContentCheck);
        }

        // POST: FunAdmin/FContentChecks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ContentId,AppellantId,ReceptionistId,DOSubmitting,DOHandling,AppellantContent,ReceptionistContent,IsLegal,ContentTitle")] FContentCheck fContentCheck)
        {
            if (id != fContentCheck.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fContentCheck);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FContentCheckExists(fContentCheck.Id))
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
            return View(fContentCheck);
        }

        // GET: FunAdmin/FContentChecks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fContentCheck = await _context.FContentCheck
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fContentCheck == null)
            {
                return NotFound();
            }

            return View(fContentCheck);
        }

        // POST: FunAdmin/FContentChecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fContentCheck = await _context.FContentCheck.FindAsync(id);
            _context.FContentCheck.Remove(fContentCheck);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FContentCheckExists(Guid id)
        {
            return _context.FContentCheck.Any(e => e.Id == id);
        }
    }
}
