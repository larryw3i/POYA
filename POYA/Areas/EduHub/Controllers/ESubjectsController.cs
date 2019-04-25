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
    public class ESubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ESubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/ESubjects
        public async Task<IActionResult> Index()
        {
            return View(await _context.ESubject.ToListAsync());
        }

        // GET: EduHub/ESubjects/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eSubject = await _context.ESubject
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eSubject == null)
            {
                return NotFound();
            }

            return View(eSubject);
        }

        // GET: EduHub/ESubjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/ESubjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] ESubject eSubject)
        {
            if (ModelState.IsValid)
            {
                eSubject.Id = Guid.NewGuid();
                _context.Add(eSubject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eSubject);
        }

        // GET: EduHub/ESubjects/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eSubject = await _context.ESubject.FindAsync(id);
            if (eSubject == null)
            {
                return NotFound();
            }
            return View(eSubject);
        }

        // POST: EduHub/ESubjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] ESubject eSubject)
        {
            if (id != eSubject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eSubject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ESubjectExists(eSubject.Id))
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
            return View(eSubject);
        }

        // GET: EduHub/ESubjects/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eSubject = await _context.ESubject
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eSubject == null)
            {
                return NotFound();
            }

            return View(eSubject);
        }

        // POST: EduHub/ESubjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eSubject = await _context.ESubject.FindAsync(id);
            _context.ESubject.Remove(eSubject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ESubjectExists(Guid id)
        {
            return _context.ESubject.Any(e => e.Id == id);
        }
    }
}
