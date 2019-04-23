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
    public class EduHubSubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EduHubSubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHubSubjects1
        public async Task<IActionResult> Index()
        {
            return View(await _context.EduHubSubject.ToListAsync());
        }

        // GET: EduHubSubjects1/Details/5
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

        // GET: EduHubSubjects1/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHubSubjects1/Create
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

        // GET: EduHubSubjects1/Edit/5
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

        // POST: EduHubSubjects1/Edit/5
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

        // GET: EduHubSubjects1/Delete/5
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

        // POST: EduHubSubjects1/Delete/5
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
