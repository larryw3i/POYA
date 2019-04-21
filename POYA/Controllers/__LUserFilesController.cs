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
    public class _LUserFilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public _LUserFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LUserFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.LUserFile.ToListAsync());
        }

        // GET: LUserFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserFile = await _context.LUserFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lUserFile == null)
            {
                return NotFound();
            }

            return View(lUserFile);
        }

        // GET: LUserFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LUserFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MD5,UserId,SharedCode,DOGenerating,Name,InDirId")] LUserFile lUserFile)
        {
            if (ModelState.IsValid)
            {
                lUserFile.Id = Guid.NewGuid();
                _context.Add(lUserFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lUserFile);
        }

        // GET: LUserFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserFile = await _context.LUserFile.FindAsync(id);
            if (lUserFile == null)
            {
                return NotFound();
            }
            return View(lUserFile);
        }

        // POST: LUserFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,MD5,UserId,SharedCode,DOGenerating,Name,InDirId")] LUserFile lUserFile)
        {
            if (id != lUserFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lUserFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LUserFileExists(lUserFile.Id))
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
            return View(lUserFile);
        }

        // GET: LUserFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserFile = await _context.LUserFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lUserFile == null)
            {
                return NotFound();
            }

            return View(lUserFile);
        }

        // POST: LUserFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lUserFile = await _context.LUserFile.FindAsync(id);
            _context.LUserFile.Remove(lUserFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LUserFileExists(Guid id)
        {
            return _context.LUserFile.Any(e => e.Id == id);
        }
    }
}
