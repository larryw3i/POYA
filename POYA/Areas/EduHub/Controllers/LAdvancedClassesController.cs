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
    public class LAdvancedClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LAdvancedClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/LAdvancedClasses
        public async Task<IActionResult> Index()
        {
            return View(await _context.LAdvancedClasses.ToListAsync());
        }

        // GET: EduHub/LAdvancedClasses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAdvancedClass = await _context.LAdvancedClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lAdvancedClass == null)
            {
                return NotFound();
            }

            return View(lAdvancedClass);
        }

        // GET: EduHub/LAdvancedClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/LAdvancedClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LFieldId,Name")] LAdvancedClass lAdvancedClass)
        {
            if (ModelState.IsValid)
            {
                lAdvancedClass.Id = Guid.NewGuid();
                _context.Add(lAdvancedClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lAdvancedClass);
        }

        // GET: EduHub/LAdvancedClasses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAdvancedClass = await _context.LAdvancedClasses.FindAsync(id);
            if (lAdvancedClass == null)
            {
                return NotFound();
            }
            return View(lAdvancedClass);
        }

        // POST: EduHub/LAdvancedClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,LFieldId,Name")] LAdvancedClass lAdvancedClass)
        {
            if (id != lAdvancedClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lAdvancedClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LAdvancedClassExists(lAdvancedClass.Id))
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
            return View(lAdvancedClass);
        }

        // GET: EduHub/LAdvancedClasses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAdvancedClass = await _context.LAdvancedClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lAdvancedClass == null)
            {
                return NotFound();
            }

            return View(lAdvancedClass);
        }

        // POST: EduHub/LAdvancedClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lAdvancedClass = await _context.LAdvancedClasses.FindAsync(id);
            _context.LAdvancedClasses.Remove(lAdvancedClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LAdvancedClassExists(Guid id)
        {
            return _context.LAdvancedClasses.Any(e => e.Id == id);
        }
    }
}
