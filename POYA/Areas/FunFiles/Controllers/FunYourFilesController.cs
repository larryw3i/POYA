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
    public class FunYourFilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FunYourFilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FunFiles/FunYourFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.FunYourFile.ToListAsync());
        }

        // GET: FunFiles/FunYourFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funYourFile = await _context.FunYourFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funYourFile == null)
            {
                return NotFound();
            }

            return View(funYourFile);
        }

        // GET: FunFiles/FunYourFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FunFiles/FunYourFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FileByteId,ParentDirId,UserId,Name,DOUploading")] FunYourFile funYourFile)
        {
            if (ModelState.IsValid)
            {
                funYourFile.Id = Guid.NewGuid();
                _context.Add(funYourFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(funYourFile);
        }

        // GET: FunFiles/FunYourFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funYourFile = await _context.FunYourFile.FindAsync(id);
            if (funYourFile == null)
            {
                return NotFound();
            }
            return View(funYourFile);
        }

        // POST: FunFiles/FunYourFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FileByteId,ParentDirId,UserId,Name,DOUploading")] FunYourFile funYourFile)
        {
            if (id != funYourFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funYourFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunYourFileExists(funYourFile.Id))
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
            return View(funYourFile);
        }

        // GET: FunFiles/FunYourFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funYourFile = await _context.FunYourFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funYourFile == null)
            {
                return NotFound();
            }

            return View(funYourFile);
        }

        // POST: FunFiles/FunYourFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var funYourFile = await _context.FunYourFile.FindAsync(id);
            _context.FunYourFile.Remove(funYourFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FunYourFileExists(Guid id)
        {
            return _context.FunYourFile.Any(e => e.Id == id);
        }
    }
}
