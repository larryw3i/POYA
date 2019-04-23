using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Data;
using POYA.Models;

namespace POYA.Controllers
{
    public class EduHubGradesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EduHubGradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHubGrades
        public async Task<IActionResult> Index()
        {
            return View(await _context.EduHubGrade.ToListAsync());
        }

        // GET: EduHubGrades/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubGrade = await _context.EduHubGrade
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eduHubGrade == null)
            {
                return NotFound();
            }

            return View(eduHubGrade);
        }

        // GET: EduHubGrades/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHubGrades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] EduHubGrade eduHubGrade)
        {
            if (ModelState.IsValid)
            {
                eduHubGrade.Id = Guid.NewGuid();
                _context.Add(eduHubGrade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eduHubGrade);
        }

        // GET: EduHubGrades/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubGrade = await _context.EduHubGrade.FindAsync(id);
            if (eduHubGrade == null)
            {
                return NotFound();
            }
            return View(eduHubGrade);
        }

        // POST: EduHubGrades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] EduHubGrade eduHubGrade)
        {
            if (id != eduHubGrade.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eduHubGrade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EduHubGradeExists(eduHubGrade.Id))
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
            return View(eduHubGrade);
        }

        // GET: EduHubGrades/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubGrade = await _context.EduHubGrade
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eduHubGrade == null)
            {
                return NotFound();
            }

            return View(eduHubGrade);
        }

        // POST: EduHubGrades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eduHubGrade = await _context.EduHubGrade.FindAsync(id);
            _context.EduHubGrade.Remove(eduHubGrade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EduHubGradeExists(Guid id)
        {
            return _context.EduHubGrade.Any(e => e.Id == id);
        }
    }
}
