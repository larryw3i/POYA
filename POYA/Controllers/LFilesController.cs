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
    public class LFilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.LFile.ToListAsync());
        }

        // GET: LFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lFile = await _context.LFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lFile == null)
            {
                return NotFound();
            }

            return View(lFile);
        }

        // GET: LFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Buffer,ContentType,FileNodeOrder,Id,UserId,InDirId,Name,DOCreate")] LFile lFile)
        {
            if (ModelState.IsValid)
            {
                lFile.Id = Guid.NewGuid();
                _context.Add(lFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lFile);
        }

        // GET: LFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lFile = await _context.LFile.FindAsync(id);
            if (lFile == null)
            {
                return NotFound();
            }
            return View(lFile);
        }

        // POST: LFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Buffer,ContentType,FileNodeOrder,Id,UserId,InDirId,Name,DOCreate")] LFile lFile)
        {
            if (id != lFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LFileExists(lFile.Id))
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
            return View(lFile);
        }

        // GET: LFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lFile = await _context.LFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lFile == null)
            {
                return NotFound();
            }

            return View(lFile);
        }

        // POST: LFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lFile = await _context.LFile.FindAsync(id);
            _context.LFile.Remove(lFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LFileExists(Guid id)
        {
            return _context.LFile.Any(e => e.Id == id);
        }
    }
}
