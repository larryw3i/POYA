using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.FunFiles.Models;
using POYA.Data;

namespace POYA.Areas.FunFiles.Controllers
{
    [Area("FunFiles")]
    public class FunDirsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FunDirsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FunFiles/FunDirs
        public async Task<IActionResult> Index()
        {
            return View(await _context.FunDir.ToListAsync());
        }

        // GET: FunFiles/FunDirs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funDir = await _context.FunDir
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funDir == null)
            {
                return NotFound();
            }

            return View(funDir);
        }

        // GET: FunFiles/FunDirs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FunFiles/FunDirs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentDirId,Name,UserId,DOCreating")] FunDir funDir)
        {
            if (ModelState.IsValid)
            {
                funDir.Id = Guid.NewGuid();
                _context.Add(funDir);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(funDir);
        }

        // GET: FunFiles/FunDirs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funDir = await _context.FunDir.FindAsync(id);
            if (funDir == null)
            {
                return NotFound();
            }
            return View(funDir);
        }

        // POST: FunFiles/FunDirs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ParentDirId,Name,UserId,DOCreating")] FunDir funDir)
        {
            if (id != funDir.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funDir);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunDirExists(funDir.Id))
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
            return View(funDir);
        }

        // GET: FunFiles/FunDirs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funDir = await _context.FunDir
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funDir == null)
            {
                return NotFound();
            }

            return View(funDir);
        }

        // POST: FunFiles/FunDirs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var funDir = await _context.FunDir.FindAsync(id);
            _context.FunDir.Remove(funDir);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FunDirExists(Guid id)
        {
            return _context.FunDir.Any(e => e.Id == id);
        }
    }
}
