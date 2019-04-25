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
    public class ETypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ETypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/ETypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.EType.ToListAsync());
        }

        // GET: EduHub/ETypes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eType = await _context.EType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eType == null)
            {
                return NotFound();
            }

            return View(eType);
        }

        // GET: EduHub/ETypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/ETypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,SubjectId")] EType eType)
        {
            if (ModelState.IsValid)
            {
                eType.Id = Guid.NewGuid();
                _context.Add(eType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eType);
        }

        // GET: EduHub/ETypes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eType = await _context.EType.FindAsync(id);
            if (eType == null)
            {
                return NotFound();
            }
            return View(eType);
        }

        // POST: EduHub/ETypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,SubjectId")] EType eType)
        {
            if (id != eType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ETypeExists(eType.Id))
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
            return View(eType);
        }

        // GET: EduHub/ETypes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eType = await _context.EType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eType == null)
            {
                return NotFound();
            }

            return View(eType);
        }

        // POST: EduHub/ETypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eType = await _context.EType.FindAsync(id);
            _context.EType.Remove(eType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ETypeExists(Guid id)
        {
            return _context.EType.Any(e => e.Id == id);
        }
    }
}
