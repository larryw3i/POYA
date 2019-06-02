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
    public class LGradesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LGradesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/LGrades
        public async Task<IActionResult> Index()
        {
            return View(await _context.LGrades.ToListAsync());
        }

        // GET: EduHub/LGrades/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lGrade = await _context.LGrades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lGrade == null)
            {
                return NotFound();
            }

            return View(lGrade);
        }

        // GET: EduHub/LGrades/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/LGrades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LSecondaryClassId,Name")] LGrade lGrade)
        {
            if (ModelState.IsValid)
            {
                lGrade.Id = Guid.NewGuid();
                _context.Add(lGrade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lGrade);
        }

        // GET: EduHub/LGrades/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lGrade = await _context.LGrades.FindAsync(id);
            if (lGrade == null)
            {
                return NotFound();
            }
            return View(lGrade);
        }

        // POST: EduHub/LGrades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,LSecondaryClassId,Name")] LGrade lGrade)
        {
            if (id != lGrade.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lGrade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LGradeExists(lGrade.Id))
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
            return View(lGrade);
        }

        // GET: EduHub/LGrades/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lGrade = await _context.LGrades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lGrade == null)
            {
                return NotFound();
            }

            return View(lGrade);
        }

        // POST: EduHub/LGrades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lGrade = await _context.LGrades.FindAsync(id);
            _context.LGrades.Remove(lGrade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LGradeExists(Guid id)
        {
            return _context.LGrades.Any(e => e.Id == id);
        }
    }
}
