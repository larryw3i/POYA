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
    public class EduHubSubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EduHubSubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/EduHubSubjects
        public async Task<IActionResult> Index()
        {
            return View(await _context.EduHubSubject.ToListAsync());
        }

        // GET: EduHub/EduHubSubjects/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubSubject = await _context.EduHubSubject
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eduHubSubject == null)
            {
                return NotFound();
            }

            return View(eduHubSubject);
        }

        // GET: EduHub/EduHubSubjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/EduHubSubjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] EduHubSubject eduHubSubject)
        {
            if (ModelState.IsValid)
            {
                eduHubSubject.Id = Guid.NewGuid();
                _context.Add(eduHubSubject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eduHubSubject);
        }

        // GET: EduHub/EduHubSubjects/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubSubject = await _context.EduHubSubject.FindAsync(id);
            if (eduHubSubject == null)
            {
                return NotFound();
            }
            return View(eduHubSubject);
        }

        // POST: EduHub/EduHubSubjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] EduHubSubject eduHubSubject)
        {
            if (id != eduHubSubject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eduHubSubject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EduHubSubjectExists(eduHubSubject.Id))
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
            return View(eduHubSubject);
        }

        // GET: EduHub/EduHubSubjects/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubSubject = await _context.EduHubSubject
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eduHubSubject == null)
            {
                return NotFound();
            }

            return View(eduHubSubject);
        }

        // POST: EduHub/EduHubSubjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eduHubSubject = await _context.EduHubSubject.FindAsync(id);
            _context.EduHubSubject.Remove(eduHubSubject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EduHubSubjectExists(Guid id)
        {
            return _context.EduHubSubject.Any(e => e.Id == id);
        }
    }
}
