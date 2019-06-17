using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.DeveloperZone.Models;
using POYA.Data;

namespace POYA.Areas.DeveloperZone.Controllers
{
    [Area("DeveloperZone")]
    public class XDeveloperNotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public XDeveloperNotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeveloperZone/XDeveloperNotes
        public async Task<IActionResult> Index()
        {
            return View(await _context.XDeveloperNote.ToListAsync());
        }

        // GET: DeveloperZone/XDeveloperNotes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloperNote = await _context.XDeveloperNote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xDeveloperNote == null)
            {
                return NotFound();
            }

            return View(xDeveloperNote);
        }

        // GET: DeveloperZone/XDeveloperNotes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeveloperZone/XDeveloperNotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,XDeveloperId,Title,Content,DOPublishing")] XDeveloperNote xDeveloperNote)
        {
            if (ModelState.IsValid)
            {
                xDeveloperNote.Id = Guid.NewGuid();
                _context.Add(xDeveloperNote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(xDeveloperNote);
        }

        // GET: DeveloperZone/XDeveloperNotes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloperNote = await _context.XDeveloperNote.FindAsync(id);
            if (xDeveloperNote == null)
            {
                return NotFound();
            }
            return View(xDeveloperNote);
        }

        // POST: DeveloperZone/XDeveloperNotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,XDeveloperId,Title,Content,DOPublishing")] XDeveloperNote xDeveloperNote)
        {
            if (id != xDeveloperNote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xDeveloperNote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XDeveloperNoteExists(xDeveloperNote.Id))
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
            return View(xDeveloperNote);
        }

        // GET: DeveloperZone/XDeveloperNotes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloperNote = await _context.XDeveloperNote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xDeveloperNote == null)
            {
                return NotFound();
            }

            return View(xDeveloperNote);
        }

        // POST: DeveloperZone/XDeveloperNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var xDeveloperNote = await _context.XDeveloperNote.FindAsync(id);
            _context.XDeveloperNote.Remove(xDeveloperNote);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool XDeveloperNoteExists(Guid id)
        {
            return _context.XDeveloperNote.Any(e => e.Id == id);
        }
    }
}
