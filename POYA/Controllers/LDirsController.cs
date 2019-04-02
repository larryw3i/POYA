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
    public class LDirsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LDirsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LDirs
        public async Task<IActionResult> Index()
        {
            return View(await _context.LDir.ToListAsync());
        }

        // GET: LDirs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lDir = await _context.LDir
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lDir == null)
            {
                return NotFound();
            }

            return View(lDir);
        }

        // GET: LDirs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LDirs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,InDirId,Name,DOCreate")] LDir lDir)
        {
            if (ModelState.IsValid)
            {
                lDir.Id = Guid.NewGuid();
                _context.Add(lDir);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lDir);
        }

        // GET: LDirs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lDir = await _context.LDir.FindAsync(id);
            if (lDir == null)
            {
                return NotFound();
            }
            return View(lDir);
        }

        // POST: LDirs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,InDirId,Name,DOCreate")] LDir lDir)
        {
            if (id != lDir.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lDir);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LDirExists(lDir.Id))
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
            return View(lDir);
        }

        // GET: LDirs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lDir = await _context.LDir
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lDir == null)
            {
                return NotFound();
            }

            return View(lDir);
        }

        // POST: LDirs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lDir = await _context.LDir.FindAsync(id);
            _context.LDir.Remove(lDir);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LDirExists(Guid id)
        {
            return _context.LDir.Any(e => e.Id == id);
        }
    }
}
