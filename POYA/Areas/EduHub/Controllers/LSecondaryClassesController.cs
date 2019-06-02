using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.EduHub.Models;
using POYA.Data;

namespace POYA.Areas.EduHub.Controllers
{
    [Area("EduHub")]
    public class LSecondaryClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LSecondaryClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/LSecondaryClasses
        public async Task<IActionResult> Index()
        {
            return View(await _context.LSecondaryClasses.ToListAsync());
        }

        // GET: EduHub/LSecondaryClasses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSecondaryClass = await _context.LSecondaryClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSecondaryClass == null)
            {
                return NotFound();
            }

            return View(lSecondaryClass);
        }

        // GET: EduHub/LSecondaryClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/LSecondaryClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LAdvancedClassId,Name")] LSecondaryClass lSecondaryClass)
        {
            if (ModelState.IsValid)
            {
                lSecondaryClass.Id = Guid.NewGuid();
                _context.Add(lSecondaryClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lSecondaryClass);
        }

        // GET: EduHub/LSecondaryClasses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSecondaryClass = await _context.LSecondaryClasses.FindAsync(id);
            if (lSecondaryClass == null)
            {
                return NotFound();
            }
            return View(lSecondaryClass);
        }

        // POST: EduHub/LSecondaryClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,LAdvancedClassId,Name")] LSecondaryClass lSecondaryClass)
        {
            if (id != lSecondaryClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lSecondaryClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LSecondaryClassExists(lSecondaryClass.Id))
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
            return View(lSecondaryClass);
        }

        // GET: EduHub/LSecondaryClasses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSecondaryClass = await _context.LSecondaryClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSecondaryClass == null)
            {
                return NotFound();
            }

            return View(lSecondaryClass);
        }

        // POST: EduHub/LSecondaryClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lSecondaryClass = await _context.LSecondaryClasses.FindAsync(id);
            _context.LSecondaryClasses.Remove(lSecondaryClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LSecondaryClassExists(Guid id)
        {
            return _context.LSecondaryClasses.Any(e => e.Id == id);
        }
    }
}
