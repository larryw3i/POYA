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
    public class LFieldsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LFieldsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/LFields
        public async Task<IActionResult> Index()
        {
            return View(await _context.LFields.ToListAsync());
        }

        // GET: EduHub/LFields/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lField = await _context.LFields
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lField == null)
            {
                return NotFound();
            }

            return View(lField);
        }

        // GET: EduHub/LFields/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/LFields/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] LField lField)
        {
            if (ModelState.IsValid)
            {
                lField.Id = Guid.NewGuid();
                _context.Add(lField);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lField);
        }

        // GET: EduHub/LFields/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lField = await _context.LFields.FindAsync(id);
            if (lField == null)
            {
                return NotFound();
            }
            return View(lField);
        }

        // POST: EduHub/LFields/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] LField lField)
        {
            if (id != lField.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lField);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LFieldExists(lField.Id))
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
            return View(lField);
        }

        // GET: EduHub/LFields/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lField = await _context.LFields
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lField == null)
            {
                return NotFound();
            }

            return View(lField);
        }

        // POST: EduHub/LFields/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lField = await _context.LFields.FindAsync(id);
            _context.LFields.Remove(lField);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LFieldExists(Guid id)
        {
            return _context.LFields.Any(e => e.Id == id);
        }
    }
}
