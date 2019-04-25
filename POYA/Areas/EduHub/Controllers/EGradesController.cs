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
    public class EGradesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EGradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/EGrades
        public async Task<IActionResult> Index()
        {
            return View(await _context.EGrade.ToListAsync());
        }

        // GET: EduHub/EGrades/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eGrade = await _context.EGrade
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eGrade == null)
            {
                return NotFound();
            }

            return View(eGrade);
        }

        // GET: EduHub/EGrades/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/EGrades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] EGrade eGrade)
        {
            if (ModelState.IsValid)
            {
                eGrade.Id = Guid.NewGuid();
                _context.Add(eGrade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eGrade);
        }

        // GET: EduHub/EGrades/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eGrade = await _context.EGrade.FindAsync(id);
            if (eGrade == null)
            {
                return NotFound();
            }
            return View(eGrade);
        }

        // POST: EduHub/EGrades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] EGrade eGrade)
        {
            if (id != eGrade.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eGrade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EGradeExists(eGrade.Id))
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
            return View(eGrade);
        }

        // GET: EduHub/EGrades/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eGrade = await _context.EGrade
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eGrade == null)
            {
                return NotFound();
            }

            return View(eGrade);
        }

        // POST: EduHub/EGrades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eGrade = await _context.EGrade.FindAsync(id);
            _context.EGrade.Remove(eGrade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EGradeExists(Guid id)
        {
            return _context.EGrade.Any(e => e.Id == id);
        }
    }
}
